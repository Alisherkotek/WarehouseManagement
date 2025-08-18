using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities;

public class ReceiptDocument : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    public ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
}