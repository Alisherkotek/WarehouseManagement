namespace WarehouseManagement.Application.DTOs;

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