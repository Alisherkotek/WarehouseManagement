namespace WarehouseManagement.Application.DTOs;

public class DocumentSummaryDto
{
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Details { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
}