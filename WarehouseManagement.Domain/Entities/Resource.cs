using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class Resource : BaseEntity, IArchivable
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    
    public ICollection<Balance> Balances { get; set; } = new List<Balance>();
    public ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
    public ICollection<ShipmentResource> ShipmentResources { get; set; } = new List<ShipmentResource>();
}