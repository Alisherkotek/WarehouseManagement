namespace WarehouseManagement.Application.DTOs;

public class ShipmentDocumentListDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public int ResourceCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public List<ShipmentResourceSummaryDto> Resources { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ShipmentResourceSummaryDto
{
    public string ResourceName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class ShipmentFilterWithPaginationDto : ShipmentFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}