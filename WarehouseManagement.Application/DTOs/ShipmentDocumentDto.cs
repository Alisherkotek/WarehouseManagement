namespace WarehouseManagement.Application.DTOs;

public class ShipmentDocumentDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();
    public List<ShipmentResourceDto> Resources { get; set; } = new();
    public decimal TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateShipmentDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public List<CreateShipmentResourceDto> Resources { get; set; } = new();
}

public class UpdateShipmentDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public List<CreateShipmentResourceDto> Resources { get; set; } = new();
}