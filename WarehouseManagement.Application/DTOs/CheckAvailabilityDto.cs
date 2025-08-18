namespace WarehouseManagement.Application.DTOs;

public class CheckAvailabilityDto
{
    public int ResourceId { get; set; }
    public int UnitOfMeasurementId { get; set; }
    public decimal RequiredQuantity { get; set; }
}