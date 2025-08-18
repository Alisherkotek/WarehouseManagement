namespace WarehouseManagement.Application.DTOs;

public class WarehouseStockDto
{
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int UnitOfMeasurementId { get; set; }
    public string UnitOfMeasurementName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public bool IsArchived { get; set; }
}