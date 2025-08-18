using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class WarehouseReportService : IWarehouseReportService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    
    public WarehouseReportService(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<WarehouseStockDto>> GetPagedStockAsync(WarehouseStockFilterDto filter)
    {
        var query = _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .AsQueryable();
            
        if (!filter.IncludeArchived)
        {
            query = query.Where(b => !b.Resource.IsArchived && !b.UnitOfMeasurement.IsArchived);
        }
        
        if (filter.ResourceIds?.Any() == true)
        {
            query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId));
        }
        
        if (filter.UnitOfMeasurementIds?.Any() == true)
        {
            query = query.Where(b => filter.UnitOfMeasurementIds.Contains(b.UnitOfMeasurementId));
        }
        
        if (!filter.IncludeZeroBalance)
        {
            query = query.Where(b => b.Quantity > 0);
        }
        
        var totalCount = await query.CountAsync();
        
        query = ApplySorting(query, filter.SortBy, filter.SortDescending);
        
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(b => new WarehouseStockDto
            {
                ResourceId = b.ResourceId,
                ResourceName = b.Resource.Name,
                UnitOfMeasurementId = b.UnitOfMeasurementId,
                UnitOfMeasurementName = b.UnitOfMeasurement.Name,
                Quantity = b.Quantity,
                IsArchived = b.Resource.IsArchived || b.UnitOfMeasurement.IsArchived
            })
            .ToListAsync();
            
        return new PagedResult<WarehouseStockDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
    
    public async Task<PagedResult<ReceiptDocumentListDto>> GetPagedReceiptsAsync(ReceiptFilterWithPaginationDto filter)
    {
        var query = _context.ReceiptDocuments
            .Include(r => r.ReceiptResources)
                .ThenInclude(rr => rr.Resource)
            .Include(r => r.ReceiptResources)
                .ThenInclude(rr => rr.UnitOfMeasurement)
            .AsQueryable();
            
        if (filter.DateFrom.HasValue)
            query = query.Where(r => r.Date >= filter.DateFrom.Value);
            
        if (filter.DateTo.HasValue)
            query = query.Where(r => r.Date <= filter.DateTo.Value);
            
        if (filter.Numbers?.Any() == true)
            query = query.Where(r => filter.Numbers.Contains(r.Number));
            
        if (filter.ResourceIds?.Any() == true)
            query = query.Where(r => r.ReceiptResources.Any(rr => filter.ResourceIds.Contains(rr.ResourceId)));
            
        if (filter.UnitOfMeasurementIds?.Any() == true)
            query = query.Where(r => r.ReceiptResources.Any(rr => filter.UnitOfMeasurementIds.Contains(rr.UnitOfMeasurementId)));
            
        var totalCount = await query.CountAsync();
        
        var sortedQuery = filter.SortBy?.ToLower() switch
        {
            "number" => filter.SortDescending ? query.OrderByDescending(r => r.Number) : query.OrderBy(r => r.Number),
            "date" => filter.SortDescending ? query.OrderByDescending(r => r.Date) : query.OrderBy(r => r.Date),
            _ => query.OrderByDescending(r => r.Date).ThenBy(r => r.Number)
        };
        
        var receipts = await sortedQuery
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
            
        var items = receipts.Select(r => new ReceiptDocumentListDto
        {
            Id = r.Id,
            Number = r.Number,
            Date = r.Date,
            ResourceCount = r.ReceiptResources.Select(rr => rr.ResourceId).Distinct().Count(),
            TotalQuantity = r.ReceiptResources.Sum(rr => rr.Quantity),
            Resources = r.ReceiptResources.Select(rr => new ReceiptResourceSummaryDto
            {
                ResourceName = rr.Resource.Name,
                UnitName = rr.UnitOfMeasurement.Name,
                Quantity = rr.Quantity
            }).ToList(),
            CreatedAt = r.CreatedAt
        }).ToList();
        
        return new PagedResult<ReceiptDocumentListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
    
    public async Task<PagedResult<ShipmentDocumentListDto>> GetPagedShipmentsAsync(ShipmentFilterWithPaginationDto filter)
    {
        var query = _context.ShipmentDocuments
            .Include(s => s.Client)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.Resource)
            .Include(s => s.ShipmentResources)
                .ThenInclude(sr => sr.UnitOfMeasurement)
            .AsQueryable();
            
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
        
        var totalCount = await query.CountAsync();
        
        var sortedQuery = filter.SortBy?.ToLower() switch
        {
            "number" => filter.SortDescending ? query.OrderByDescending(s => s.Number) : query.OrderBy(s => s.Number),
            "date" => filter.SortDescending ? query.OrderByDescending(s => s.Date) : query.OrderBy(s => s.Date),
            "client" => filter.SortDescending ? query.OrderByDescending(s => s.Client.Name) : query.OrderBy(s => s.Client.Name),
            "status" => filter.SortDescending ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            _ => query.OrderByDescending(s => s.Date).ThenBy(s => s.Number)
        };
        
        var shipments = await sortedQuery
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
            
        var items = shipments.Select(s => new ShipmentDocumentListDto
        {
            Id = s.Id,
            Number = s.Number,
            Date = s.Date,
            ClientName = s.Client.Name,
            Status = (DTOs.ShipmentStatus)(int)s.Status,
            StatusDisplay = s.Status.ToString(),
            ResourceCount = s.ShipmentResources.Select(sr => sr.ResourceId).Distinct().Count(),
            TotalQuantity = s.ShipmentResources.Sum(sr => sr.Quantity),
            Resources = s.ShipmentResources.Select(sr => new ShipmentResourceSummaryDto
            {
                ResourceName = sr.Resource.Name,
                UnitName = sr.UnitOfMeasurement.Name,
                Quantity = sr.Quantity
            }).ToList(),
            CreatedAt = s.CreatedAt
        }).ToList();
        
        return new PagedResult<ShipmentDocumentListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
    
    public async Task<IEnumerable<DocumentSummaryDto>> GetRecentDocumentsAsync(int count = 10)
    {
        var recentReceipts = await _context.ReceiptDocuments
            .OrderByDescending(r => r.Date)
            .Take(count)
            .Select(r => new DocumentSummaryDto
            {
                DocumentType = "Receipt",
                DocumentNumber = r.Number,
                Date = r.Date,
                Details = $"{r.ReceiptResources.Count} items",
                TotalQuantity = r.ReceiptResources.Sum(rr => rr.Quantity),
                Status = "Completed"
            })
            .ToListAsync();
            
        var recentShipments = await _context.ShipmentDocuments
            .Include(s => s.Client)
            .OrderByDescending(s => s.Date)
            .Take(count)
            .Select(s => new DocumentSummaryDto
            {
                DocumentType = "Shipment",
                DocumentNumber = s.Number,
                Date = s.Date,
                Details = s.Client.Name,
                TotalQuantity = s.ShipmentResources.Sum(sr => sr.Quantity),
                Status = s.Status.ToString()
            })
            .ToListAsync();
            
        return recentReceipts.Concat(recentShipments)
            .OrderByDescending(d => d.Date)
            .Take(count);
    }
    
    public async Task<Dictionary<string, object>> GetWarehouseSummaryAsync()
    {
        var totalResources = await _context.Resources.CountAsync(r => !r.IsArchived);
        var totalClients = await _context.Clients.CountAsync(c => !c.IsArchived);
        var totalReceipts = await _context.ReceiptDocuments.CountAsync();
        var totalShipments = await _context.ShipmentDocuments.CountAsync();
        var totalStock = await _context.Balances.SumAsync(b => b.Quantity);
        
        var topResources = await _context.Balances
            .Include(b => b.Resource)
            .Include(b => b.UnitOfMeasurement)
            .Where(b => b.Quantity > 0)
            .OrderByDescending(b => b.Quantity)
            .Take(5)
            .Select(b => new
            {
                ResourceName = b.Resource.Name,
                UnitName = b.UnitOfMeasurement.Name,
                Quantity = b.Quantity
            })
            .ToListAsync();
            
        return new Dictionary<string, object>
        {
            ["totalResources"] = totalResources,
            ["totalClients"] = totalClients,
            ["totalReceipts"] = totalReceipts,
            ["totalShipments"] = totalShipments,
            ["totalStock"] = totalStock,
            ["topResources"] = topResources
        };
    }
    
    private IQueryable<Domain.Entities.Balance> ApplySorting(
        IQueryable<Domain.Entities.Balance> query, 
        string? sortBy, 
        bool descending)
    {
        return sortBy?.ToLower() switch
        {
            "resource" => descending ? 
                query.OrderByDescending(b => b.Resource.Name) : 
                query.OrderBy(b => b.Resource.Name),
            "unit" => descending ? 
                query.OrderByDescending(b => b.UnitOfMeasurement.Name) : 
                query.OrderBy(b => b.UnitOfMeasurement.Name),
            "quantity" => descending ? 
                query.OrderByDescending(b => b.Quantity) : 
                query.OrderBy(b => b.Quantity),
            _ => query.OrderBy(b => b.Resource.Name).ThenBy(b => b.UnitOfMeasurement.Name)
        };
    }
}