using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IWarehouseReportService _reportService;

    public ReportsController(IWarehouseReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("warehouse-stock")]
    public async Task<ActionResult<PagedResult<WarehouseStockDto>>> GetPagedStock(
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] bool includeZeroBalance = false,
        [FromQuery] bool includeArchived = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var filter = new WarehouseStockFilterDto
        {
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            IncludeZeroBalance = includeZeroBalance,
            IncludeArchived = includeArchived,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _reportService.GetPagedStockAsync(filter);
        return Ok(result);
    }

    [HttpGet("receipts")]
    public async Task<ActionResult<PagedResult<ReceiptDocumentListDto>>> GetPagedReceipts(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] List<string>? numbers = null,
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var filter = new ReceiptFilterWithPaginationDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Numbers = numbers,
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _reportService.GetPagedReceiptsAsync(filter);
        return Ok(result);
    }

    [HttpGet("shipments")]
    public async Task<ActionResult<PagedResult<ShipmentDocumentListDto>>> GetPagedShipments(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] List<string>? numbers = null,
        [FromQuery] List<int>? clientIds = null,
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] List<ShipmentStatus>? statuses = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var filter = new ShipmentFilterWithPaginationDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Numbers = numbers,
            ClientIds = clientIds,
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            Statuses = statuses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _reportService.GetPagedShipmentsAsync(filter);
        return Ok(result);
    }

    [HttpGet("recent-documents")]
    public async Task<ActionResult<IEnumerable<DocumentSummaryDto>>> GetRecentDocuments([FromQuery] int count = 10)
    {
        var documents = await _reportService.GetRecentDocumentsAsync(count);
        return Ok(documents);
    }

    [HttpGet("warehouse-summary")]
    public async Task<ActionResult<Dictionary<string, object>>> GetWarehouseSummary()
    {
        var summary = await _reportService.GetWarehouseSummaryAsync();
        return Ok(summary);
    }
}