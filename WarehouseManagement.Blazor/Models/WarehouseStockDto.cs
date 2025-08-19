namespace WarehouseManagement.Blazor.Models;

public class WarehouseStockDto
{
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int UnitOfMeasurementId { get; set; }
    public string UnitOfMeasurementName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public bool IsArchived { get; set; }
}

public class WarehouseStockFilterDto
{
    public List<int>? ResourceIds { get; set; }
    public List<int>? UnitOfMeasurementIds { get; set; }
    public bool IncludeZeroBalance { get; set; } = false;
    public bool IncludeArchived { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}