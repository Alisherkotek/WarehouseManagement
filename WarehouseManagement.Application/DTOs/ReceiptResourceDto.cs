namespace WarehouseManagement.Application.DTOs;

public class ReceiptResourceDto
{
    public int Id { get; set; }
    public int ReceiptDocumentId { get; set; }
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int UnitOfMeasurementId { get; set; }
    public string UnitOfMeasurementName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class CreateReceiptResourceDto
{
    public int ResourceId { get; set; }
    public int UnitOfMeasurementId { get; set; }
    public decimal Quantity { get; set; }
}