namespace WarehouseManagement.Application.DTOs;

public class ResourceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateResourceDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateResourceDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}