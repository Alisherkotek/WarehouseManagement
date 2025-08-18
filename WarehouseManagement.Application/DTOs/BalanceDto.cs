namespace WarehouseManagement.Application.DTOs;

public class BalanceDto
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int UnitOfMeasurementId { get; set; }
    public string UnitOfMeasurementName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BalanceFilterDto
{
    public List<int>? ResourceIds { get; set; }
    public List<int>? UnitOfMeasurementIds { get; set; }
    public bool IncludeZeroBalance { get; set; } = false;
}