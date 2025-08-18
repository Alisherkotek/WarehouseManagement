using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class ShipmentDocument : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
    public ShipmentStatus Status { get; set; }
    
    public ICollection<ShipmentResource> ShipmentResources { get; set; } = new List<ShipmentResource>();
}