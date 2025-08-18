using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class ReceiptDocumentService : IReceiptDocumentService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBalanceService _balanceService;

    public ReceiptDocumentService(
        WarehouseDbContext context,
        IMapper mapper,
        IBalanceService balanceService)
    {
        _context = context;
        _mapper = mapper;
        _balanceService = balanceService;
    }

    public async Task<IEnumerable<ReceiptDocumentDto>> GetAllAsync(ReceiptFilterDto? filter = null)
    {
        var query = _context.ReceiptDocuments
            .Include(r => r.ReceiptResources)
            .ThenInclude(rr => rr.Resource)
            .Include(r => r.ReceiptResources)
            .ThenInclude(rr => rr.UnitOfMeasurement)
            .AsQueryable();

        if (filter != null)
        {
            if (filter.DateFrom.HasValue)
                query = query.Where(r => r.Date >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(r => r.Date <= filter.DateTo.Value);

            if (filter.Numbers?.Any() == true)
                query = query.Where(r => filter.Numbers.Contains(r.Number));

            if (filter.ResourceIds?.Any() == true)
                query = query.Where(r => r.ReceiptResources.Any(rr => filter.ResourceIds.Contains(rr.ResourceId)));

            if (filter.UnitOfMeasurementIds?.Any() == true)
                query = query.Where(r =>
                    r.ReceiptResources.Any(rr => filter.UnitOfMeasurementIds.Contains(rr.UnitOfMeasurementId)));
        }

        var receipts = await query.OrderByDescending(r => r.Date).ThenBy(r => r.Number).ToListAsync();

        return receipts.Select(MapToDto);
    }

    public async Task<ReceiptDocumentDto?> GetByIdAsync(int id)
    {
        var receipt = await _context.ReceiptDocuments
            .Include(r => r.ReceiptResources)
            .ThenInclude(rr => rr.Resource)
            .Include(r => r.ReceiptResources)
            .ThenInclude(rr => rr.UnitOfMeasurement)
            .FirstOrDefaultAsync(r => r.Id == id);

        return receipt == null ? null : MapToDto(receipt);
    }

    public async Task<ReceiptDocumentDto> CreateAsync(CreateReceiptDocumentDto dto)
    {
        var exists = await _context.ReceiptDocuments.AnyAsync(r => r.Number == dto.Number);
        if (exists)
            throw new DuplicateEntityException("Receipt Document", "number", dto.Number);

        await ValidateResourcesAsync(dto.Resources);

        var receipt = new ReceiptDocument
        {
            Number = dto.Number,
            Date = dto.Date,
            ReceiptResources = dto.Resources.Select(r => new ReceiptResource
            {
                ResourceId = r.ResourceId,
                UnitOfMeasurementId = r.UnitOfMeasurementId,
                Quantity = r.Quantity
            }).ToList()
        };

        _context.ReceiptDocuments.Add(receipt);
        await _context.SaveChangesAsync();

        foreach (var resource in receipt.ReceiptResources)
        {
            await _balanceService.AdjustBalanceAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                resource.Quantity,
                $"Receipt {receipt.Number}");
        }

        await _context.Entry(receipt).Collection(r => r.ReceiptResources).LoadAsync();
        foreach (var resource in receipt.ReceiptResources)
        {
            await _context.Entry(resource).Reference(r => r.Resource).LoadAsync();
            await _context.Entry(resource).Reference(r => r.UnitOfMeasurement).LoadAsync();
        }

        return MapToDto(receipt);
    }

    public async Task<ReceiptDocumentDto> UpdateAsync(int id, UpdateReceiptDocumentDto dto)
    {
        var receipt = await _context.ReceiptDocuments
            .Include(r => r.ReceiptResources)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null)
            throw new EntityNotFoundException("Receipt Document", id);

        var duplicateExists = await _context.ReceiptDocuments
            .AnyAsync(r => r.Number == dto.Number && r.Id != id);
        if (duplicateExists)
            throw new DuplicateEntityException("Receipt Document", "number", dto.Number);

        await ValidateResourcesAsync(dto.Resources);

        foreach (var oldResource in receipt.ReceiptResources)
        {
            await _balanceService.AdjustBalanceAsync(
                oldResource.ResourceId,
                oldResource.UnitOfMeasurementId,
                -oldResource.Quantity,
                $"Receipt {receipt.Number} (reversal for update)");
        }

        receipt.Number = dto.Number;
        receipt.Date = dto.Date;

        _context.ReceiptResources.RemoveRange(receipt.ReceiptResources);
        receipt.ReceiptResources = dto.Resources.Select(r => new ReceiptResource
        {
            ReceiptDocumentId = id,
            ResourceId = r.ResourceId,
            UnitOfMeasurementId = r.UnitOfMeasurementId,
            Quantity = r.Quantity
        }).ToList();

        await _context.SaveChangesAsync();

        foreach (var resource in receipt.ReceiptResources)
        {
            await _balanceService.AdjustBalanceAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                resource.Quantity,
                $"Receipt {receipt.Number} (updated)");
        }

        foreach (var resource in receipt.ReceiptResources)
        {
            await _context.Entry(resource).Reference(r => r.Resource).LoadAsync();
            await _context.Entry(resource).Reference(r => r.UnitOfMeasurement).LoadAsync();
        }

        return MapToDto(receipt);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var receipt = await _context.ReceiptDocuments
            .Include(r => r.ReceiptResources)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null)
            throw new EntityNotFoundException("Receipt Document", id);

        foreach (var resource in receipt.ReceiptResources)
        {
            var currentQuantity = await _balanceService.GetQuantityAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId);

            if (currentQuantity < resource.Quantity)
            {
                var res = await _context.Resources.FindAsync(resource.ResourceId);
                throw new InsufficientStockException(
                    res?.Name ?? "Resource",
                    resource.Quantity,
                    currentQuantity);
            }
        }

        foreach (var resource in receipt.ReceiptResources)
        {
            await _balanceService.AdjustBalanceAsync(
                resource.ResourceId,
                resource.UnitOfMeasurementId,
                -resource.Quantity,
                $"Receipt {receipt.Number} (deleted)");
        }

        _context.ReceiptDocuments.Remove(receipt);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ValidateResourcesAsync(List<CreateReceiptResourceDto> resources)
    {
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

    private ReceiptDocumentDto MapToDto(ReceiptDocument receipt)
    {
        return new ReceiptDocumentDto
        {
            Id = receipt.Id,
            Number = receipt.Number,
            Date = receipt.Date,
            Resources = receipt.ReceiptResources.Select(r => new ReceiptResourceDto
            {
                Id = r.Id,
                ReceiptDocumentId = r.ReceiptDocumentId,
                ResourceId = r.ResourceId,
                ResourceName = r.Resource?.Name ?? string.Empty,
                UnitOfMeasurementId = r.UnitOfMeasurementId,
                UnitOfMeasurementName = r.UnitOfMeasurement?.Name ?? string.Empty,
                Quantity = r.Quantity
            }).ToList(),
            TotalItems = receipt.ReceiptResources.Sum(r => r.Quantity),
            CreatedAt = receipt.CreatedAt,
            UpdatedAt = receipt.UpdatedAt
        };
    }
}