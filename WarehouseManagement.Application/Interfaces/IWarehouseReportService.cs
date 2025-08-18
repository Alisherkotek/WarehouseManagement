namespace WarehouseManagement.Application.Interfaces;

using WarehouseManagement.Application.DTOs;

public interface IWarehouseReportService
{
    Task<PagedResult<WarehouseStockDto>> GetPagedStockAsync(WarehouseStockFilterDto filter);
    Task<PagedResult<ReceiptDocumentListDto>> GetPagedReceiptsAsync(ReceiptFilterWithPaginationDto filter);
    Task<PagedResult<ShipmentDocumentListDto>> GetPagedShipmentsAsync(ShipmentFilterWithPaginationDto filter);
    Task<IEnumerable<DocumentSummaryDto>> GetRecentDocumentsAsync(int count = 10);
    Task<Dictionary<string, object>> GetWarehouseSummaryAsync();
}