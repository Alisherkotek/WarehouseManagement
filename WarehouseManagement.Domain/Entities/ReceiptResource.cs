using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class ReceiptResource : BaseEntity
{
    public int ReceiptDocumentId { get; set; }
    public ReceiptDocument ReceiptDocument { get; set; } = null!;
    
    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
    
    public int UnitOfMeasurementId { get; set; }
    public UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;
    
    public decimal Quantity { get; set; }
}