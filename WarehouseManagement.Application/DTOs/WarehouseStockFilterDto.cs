namespace WarehouseManagement.Application.DTOs;

public class WarehouseStockFilterDto : PaginationParams
{
    public List<int>? ResourceIds { get; set; }
    public List<int>? UnitOfMeasurementIds { get; set; }
    public bool IncludeZeroBalance { get; set; } = false;
    public bool IncludeArchived { get; set; } = false;
}

public class ReceiptDocumentListDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ResourceCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public List<ReceiptResourceSummaryDto> Resources { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ReceiptResourceSummaryDto
{
    public string ResourceName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class ReceiptFilterWithPaginationDto : ReceiptFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}