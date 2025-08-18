namespace WarehouseManagement.Application.DTOs;

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
    public DateTime Date { get; set; }
    public List<CreateReceiptResourceDto> Resources { get; set; } = new();
}

public class UpdateReceiptDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<CreateReceiptResourceDto> Resources { get; set; } = new();
}