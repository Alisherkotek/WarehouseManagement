namespace WarehouseManagement.Blazor.Models;

public class ReceiptDocumentDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<ReceiptResourceDto> Resources { get; set; } = new();
    public decimal TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateReceiptDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public List<CreateReceiptResourceDto> Resources { get; set; } = new();
}

public class UpdateReceiptDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<CreateReceiptResourceDto> Resources { get; set; } = new();
}

public class ReceiptResourceDto
{
    public int Id { get; set; }
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

public class ReceiptFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<string>? Numbers { get; set; }
    public List<int>? ResourceIds { get; set; }
    public List<int>? UnitOfMeasurementIds { get; set; }
}