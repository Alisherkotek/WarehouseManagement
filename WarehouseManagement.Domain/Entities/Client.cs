using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class Client : BaseEntity, IArchivable
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    
    public ICollection<ShipmentDocument> ShipmentDocuments { get; set; } = new List<ShipmentDocument>();
}