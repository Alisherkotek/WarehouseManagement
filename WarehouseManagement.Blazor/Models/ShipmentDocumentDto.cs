namespace WarehouseManagement.Blazor.Models;

public class ShipmentDocumentDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; }
    public List<ShipmentResourceDto> Resources { get; set; } = new();
    public decimal TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateShipmentDocumentDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
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

public class ShipmentResourceDto
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public int UnitOfMeasurementId { get; set; }
    public string UnitOfMeasurementName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class CreateShipmentResourceDto
{
    public int ResourceId { get; set; }
    public int UnitOfMeasurementId { get; set; }
    public decimal Quantity { get; set; }
}

public class ShipmentFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<string>? Numbers { get; set; }
    public List<int>? ClientIds { get; set; }
    public List<int>? ResourceIds { get; set; }
    public List<int>? UnitOfMeasurementIds { get; set; }
    public List<ShipmentStatus>? Statuses { get; set; }
}

public enum ShipmentStatus
{
    Draft = 0,
    Signed = 1,
    Cancelled = 2
}