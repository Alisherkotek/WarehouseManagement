using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class ShipmentDocumentService : IShipmentDocumentService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBalanceService _balanceService;
    
    public ShipmentDocumentService(
        WarehouseDbContext context, 
        IMapper mapper,
        IBalanceService balanceService)
    {
        _context = context;
        _mapper = mapper;
        _balanceService = balanceService;
    }
    
    public async Task<IEnumerable<ShipmentDocumentDto>> GetAllAsync(ShipmentFilterDto? filter = null)
    {
        var query = _context.ShipmentDocuments
            .Include(s => s.Client)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.Resource)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.UnitOfMeasurement)
            .AsQueryable();
            
        if (filter != null)
        {
            if (filter.DateFrom.HasValue)
                query = query.Where(s => s.Date >= filter.DateFrom.Value);
                
            if (filter.DateTo.HasValue)
                query = query.Where(s => s.Date <= filter.DateTo.Value);
                
            if (filter.Numbers?.Any() == true)
                query = query.Where(s => filter.Numbers.Contains(s.Number));
                
            if (filter.ClientIds?.Any() == true)
                query = query.Where(s => filter.ClientIds.Contains(s.ClientId));
                
            if (filter.ResourceIds?.Any() == true)
                query = query.Where(s => s.ShipmentResources.Any(sr => filter.ResourceIds.Contains(sr.ResourceId)));
                
            if (filter.UnitOfMeasurementIds?.Any() == true)
                query = query.Where(s => s.ShipmentResources.Any(sr => filter.UnitOfMeasurementIds.Contains(sr.UnitOfMeasurementId)));
                
            if (filter.Statuses?.Any() == true)
            {
                var domainStatuses = filter.Statuses.Select(s => (Domain.Entities.ShipmentStatus)(int)s).ToList();
                query = query.Where(s => domainStatuses.Contains(s.Status));
            }
        }
        
        var shipments = await query.OrderByDescending(s => s.Date).ThenBy(s => s.Number).ToListAsync();
        
        return shipments.Select(MapToDto);
    }
    
    public async Task<ShipmentDocumentDto?> GetByIdAsync(int id)
    {
        var shipment = await GetShipmentWithIncludesAsync(id);
        return shipment == null ? null : MapToDto(shipment);
    }
    
    public async Task<ShipmentDocumentDto> CreateAsync(CreateShipmentDocumentDto dto)
    {
        if (!dto.Resources.Any())
            throw new BusinessException("Shipment document cannot be empty");
            
        var exists = await _context.ShipmentDocuments.AnyAsync(s => s.Number == dto.Number);
        if (exists)
            throw new DuplicateEntityException("Shipment Document", "number", dto.Number);
            
        await ValidateReferencesAsync(dto.ClientId, dto.Resources);
        
        var shipment = new ShipmentDocument
        {
            Number = dto.Number,
            Date = dto.Date,
            ClientId = dto.ClientId,
            Status = Domain.Entities.ShipmentStatus.Draft,
            ShipmentResources = dto.Resources.Select(r => new ShipmentResource
            {
                ResourceId = r.ResourceId,
                UnitOfMeasurementId = r.UnitOfMeasurementId,
                Quantity = r.Quantity
            }).ToList()
        };
        
        _context.ShipmentDocuments.Add(shipment);
        await _context.SaveChangesAsync();
        
        await LoadRelatedDataAsync(shipment);
        return MapToDto(shipment);
    }
    
    public async Task<ShipmentDocumentDto> UpdateAsync(int id, UpdateShipmentDocumentDto dto)
    {
        if (!dto.Resources.Any())
            throw new BusinessException("Shipment document cannot be empty");
            
        var shipment = await _context.ShipmentDocuments
            .Include(s => s.ShipmentResources)
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (shipment == null)
            throw new EntityNotFoundException("Shipment Document", id);
            
        if (shipment.Status == Domain.Entities.ShipmentStatus.Signed)
            throw new BusinessException("Cannot update signed shipment document");
            
        var duplicateExists = await _context.ShipmentDocuments
            .AnyAsync(s => s.Number == dto.Number && s.Id != id);
        if (duplicateExists)
            throw new DuplicateEntityException("Shipment Document", "number", dto.Number);
            
        await ValidateReferencesAsync(dto.ClientId, dto.Resources);
        
        shipment.Number = dto.Number;
        shipment.Date = dto.Date;
        shipment.ClientId = dto.ClientId;
        
        _context.ShipmentResources.RemoveRange(shipment.ShipmentResources);
        shipment.ShipmentResources = dto.Resources.Select(r => new ShipmentResource
        {
            ShipmentDocumentId = id,
            ResourceId = r.ResourceId,
            UnitOfMeasurementId = r.UnitOfMeasurementId,
            Quantity = r.Quantity
        }).ToList();
        
        await _context.SaveChangesAsync();
        
        await LoadRelatedDataAsync(shipment);
        return MapToDto(shipment);
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var shipment = await _context.ShipmentDocuments
            .Include(s => s.ShipmentResources)
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (shipment == null)
            throw new EntityNotFoundException("Shipment Document", id);
            
        if (shipment.Status == Domain.Entities.ShipmentStatus.Signed)
        {
            foreach (var resource in shipment.ShipmentResources)
            {
                await _balanceService.AdjustBalanceAsync(
                    resource.ResourceId,
                    resource.UnitOfMeasurementId,
                    resource.Quantity,
                    $"Shipment {shipment.Number} (deleted after signing)");
            }
        }
        
        _context.ShipmentDocuments.Remove(shipment);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<ShipmentDocumentDto> SignAsync(int id)
    {
        var shipment = await GetShipmentWithIncludesAsync(id);
        
        if (shipment == null)
            throw new EntityNotFoundException("Shipment Document", id);
            
        if (shipment.Status == Domain.Entities.ShipmentStatus.Signed)
            throw new BusinessException("Shipment document is already signed");
            
        foreach (var resource in shipment.ShipmentResources)
        {
            var hasQuantity = await _balanceService.HasSufficientQuantityAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                resource.Quantity);
                
            if (!hasQuantity)
            {
                var res = await _context.Resources.FindAsync(resource.ResourceId);
                var available = await _balanceService.GetQuantityAsync(resource.ResourceId, resource.UnitOfMeasurementId);
                throw new InsufficientStockException(res?.Name ?? "Resource", resource.Quantity, available);
            }
        }
        
        foreach (var resource in shipment.ShipmentResources)
        {
            await _balanceService.AdjustBalanceAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                -resource.Quantity,
                $"Shipment {shipment.Number} (signed)");
        }
        
        shipment.Status = Domain.Entities.ShipmentStatus.Signed;
        await _context.SaveChangesAsync();
        
        return MapToDto(shipment);
    }
    
    public async Task<ShipmentDocumentDto> CancelAsync(int id)
    {
        var shipment = await GetShipmentWithIncludesAsync(id);
        
        if (shipment == null)
            throw new EntityNotFoundException("Shipment Document", id);
            
        if (shipment.Status != Domain.Entities.ShipmentStatus.Signed)
            throw new BusinessException("Only signed shipment documents can be cancelled");
            
        foreach (var resource in shipment.ShipmentResources)
        {
            await _balanceService.AdjustBalanceAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                resource.Quantity,
                $"Shipment {shipment.Number} (cancelled)");
        }
        
        shipment.Status = Domain.Entities.ShipmentStatus.Cancelled;
        await _context.SaveChangesAsync();
        
        return MapToDto(shipment);
    }
    
    private async Task ValidateReferencesAsync(int clientId, List<CreateShipmentResourceDto> resources)
    {
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
        if (!clientExists)
            throw new EntityNotFoundException("Client", clientId);
            
        foreach (var resource in resources)
        {
            var resourceExists = await _context.Resources.AnyAsync(r => r.Id == resource.ResourceId);
            if (!resourceExists)
                throw new EntityNotFoundException("Resource", resource.ResourceId);
                
            var unitExists = await _context.UnitsOfMeasurement.AnyAsync(u => u.Id == resource.UnitOfMeasurementId);
            if (!unitExists)
                throw new EntityNotFoundException("Unit of Measurement", resource.UnitOfMeasurementId);
                
            if (resource.Quantity <= 0)
                throw new BusinessException("Quantity must be greater than zero");
        }
    }
    
    private async Task<ShipmentDocument?> GetShipmentWithIncludesAsync(int id)
    {
        return await _context.ShipmentDocuments
            .Include(s => s.Client)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.Resource)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.UnitOfMeasurement)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    private async Task LoadRelatedDataAsync(ShipmentDocument shipment)
    {
        await _context.Entry(shipment).Reference(s => s.Client).LoadAsync();
        await _context.Entry(shipment).Collection(s => s.ShipmentResources).LoadAsync();
        foreach (var resource in shipment.ShipmentResources)
        {
            await _context.Entry(resource).Reference(r => r.Resource).LoadAsync();
            await _context.Entry(resource).Reference(r => r.UnitOfMeasurement).LoadAsync();
        }
    }
    
    private ShipmentDocumentDto MapToDto(ShipmentDocument shipment)
    {
        return new ShipmentDocumentDto
        {
            Id = shipment.Id,
            Number = shipment.Number,
            Date = shipment.Date,
            ClientId = shipment.ClientId,
            ClientName = shipment.Client?.Name ?? string.Empty,
            Status = (DTOs.ShipmentStatus)(int)shipment.Status,
            Resources = shipment.ShipmentResources.Select(r => new ShipmentResourceDto
            {
                Id = r.Id,
                ShipmentDocumentId = r.ShipmentDocumentId,
                ResourceId = r.ResourceId,
                ResourceName = r.Resource?.Name ?? string.Empty,
                UnitOfMeasurementId = r.UnitOfMeasurementId,
                UnitOfMeasurementName = r.UnitOfMeasurement?.Name ?? string.Empty,
                Quantity = r.Quantity
            }).ToList(),
            TotalItems = shipment.ShipmentResources.Sum(r => r.Quantity),
            CreatedAt = shipment.CreatedAt,
            UpdatedAt = shipment.UpdatedAt
        };
    }
}