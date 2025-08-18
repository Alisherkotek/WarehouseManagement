using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class ShipmentResource : BaseEntity
{
    public int ShipmentDocumentId { get; set; }
    public ShipmentDocument ShipmentDocument { get; set; } = null!;
    
    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
    
    public int UnitOfMeasurementId { get; set; }
    public UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;
    
    public decimal Quantity { get; set; }
}