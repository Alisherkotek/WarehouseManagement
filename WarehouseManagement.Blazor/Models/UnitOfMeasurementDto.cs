namespace WarehouseManagement.Blazor.Models;

public class UnitOfMeasurementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUnitOfMeasurementDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateUnitOfMeasurementDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}