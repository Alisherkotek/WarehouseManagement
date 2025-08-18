using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    
    public BalanceService(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(BalanceFilterDto? filter = null)
    {
        var query = _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .AsQueryable();
            
        if (filter != null)
        {
            if (filter.ResourceIds?.Any() == true)
                query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId));
                
            if (filter.UnitOfMeasurementIds?.Any() == true)
                query = query.Where(b => filter.UnitOfMeasurementIds.Contains(b.UnitOfMeasurementId));
                
            if (!filter.IncludeZeroBalance)
                query = query.Where(b => b.Quantity > 0);
        }
        
        var balances = await query.OrderBy(b => b.Resource.Name)
            .ThenBy(b => b.UnitOfMeasurement.Name)
            .ToListAsync();
            
        return balances.Select(b => new BalanceDto
        {
            Id = b.Id,
            ResourceId = b.ResourceId,
            ResourceName = b.Resource.Name,
            UnitOfMeasurementId = b.UnitOfMeasurementId,
            UnitOfMeasurementName = b.UnitOfMeasurement.Name,
            Quantity = b.Quantity,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        });
    }
    
    public async Task<BalanceDto?> GetBalanceAsync(int resourceId, int unitOfMeasurementId)
    {
        var balance = await _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .FirstOrDefaultAsync(b => b.ResourceId == resourceId && 
                                     b.UnitOfMeasurementId == unitOfMeasurementId);
            
        if (balance == null)
            return null;
            
        return new BalanceDto
        {
            Id = balance.Id,
            ResourceId = balance.ResourceId,
            ResourceName = balance.Resource.Name,
            UnitOfMeasurementId = balance.UnitOfMeasurementId,
            UnitOfMeasurementName = balance.UnitOfMeasurement.Name,
            Quantity = balance.Quantity,
            CreatedAt = balance.CreatedAt,
            UpdatedAt = balance.UpdatedAt
        };
    }
    
    public async Task<decimal> GetQuantityAsync(int resourceId, int unitOfMeasurementId)
    {
        var balance = await _context.Balances
            .FirstOrDefaultAsync(b => b.ResourceId == resourceId && 
                                     b.UnitOfMeasurementId == unitOfMeasurementId);
            
        return balance?.Quantity ?? 0;
    }
    
    public async Task<bool> HasSufficientQuantityAsync(int resourceId, int unitOfMeasurementId, decimal requiredQuantity)
    {
        var availableQuantity = await GetQuantityAsync(resourceId, unitOfMeasurementId);
        return availableQuantity >= requiredQuantity;
    }
    
    public async Task<BalanceDto> AdjustBalanceAsync(int resourceId, int unitOfMeasurementId, decimal quantityChange, string reason)
    {
        var resource = await _context.Resources.FindAsync(resourceId);
        if (resource == null)
            throw new EntityNotFoundException("Resource", resourceId);
            
        var unit = await _context.UnitsOfMeasurement.FindAsync(unitOfMeasurementId);
        if (unit == null)
            throw new EntityNotFoundException("Unit of Measurement", unitOfMeasurementId);
            
        var balance = await _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .FirstOrDefaultAsync(b => b.ResourceId == resourceId && 
                                     b.UnitOfMeasurementId == unitOfMeasurementId);
            
        if (balance == null)
        {
            if (quantityChange < 0)
                throw new InsufficientStockException(resource.Name, Math.Abs(quantityChange), 0);
                
            balance = new Balance
            {
                ResourceId = resourceId,
                UnitOfMeasurementId = unitOfMeasurementId,
                Quantity = quantityChange
            };
            _context.Balances.Add(balance);
        }
        else
        {
            var newQuantity = balance.Quantity + quantityChange;
            if (newQuantity < 0)
                throw new InsufficientStockException(resource.Name, Math.Abs(quantityChange), balance.Quantity);
                
            balance.Quantity = newQuantity;
        }
        
        await _context.SaveChangesAsync();
        
        await _context.Entry(balance).Reference(b => b.Resource).LoadAsync();
        await _context.Entry(balance).Reference(b => b.UnitOfMeasurement).LoadAsync();
        
        return new BalanceDto
        {
            Id = balance.Id,
            ResourceId = balance.ResourceId,
            ResourceName = balance.Resource.Name,
            UnitOfMeasurementId = balance.UnitOfMeasurementId,
            UnitOfMeasurementName = balance.UnitOfMeasurement.Name,
            Quantity = balance.Quantity,
            CreatedAt = balance.CreatedAt,
            UpdatedAt = balance.UpdatedAt
        };
    }
    
    public async Task<IEnumerable<WarehouseStockDto>> GetWarehouseStockAsync(BalanceFilterDto? filter = null)
    {
        var query = _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .AsQueryable();
            
        if (filter != null)
        {
            if (filter.ResourceIds?.Any() == true)
                query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId));
                
            if (filter.UnitOfMeasurementIds?.Any() == true)
                query = query.Where(b => filter.UnitOfMeasurementIds.Contains(b.UnitOfMeasurementId));
                
            if (!filter.IncludeZeroBalance)
                query = query.Where(b => b.Quantity > 0);
        }
        
        var balances = await query
            .OrderBy(b => b.Resource.Name)
            .ThenBy(b => b.UnitOfMeasurement.Name)
            .ToListAsync();
            
        return balances.Select(b => new WarehouseStockDto
        {
            ResourceId = b.ResourceId,
            ResourceName = b.Resource.Name,
            UnitOfMeasurementId = b.UnitOfMeasurementId,
            UnitOfMeasurementName = b.UnitOfMeasurement.Name,
            Quantity = b.Quantity,
            IsArchived = b.Resource.IsArchived || b.UnitOfMeasurement.IsArchived
        });
    }
}