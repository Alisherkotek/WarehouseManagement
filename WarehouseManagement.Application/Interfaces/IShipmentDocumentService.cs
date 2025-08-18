namespace WarehouseManagement.Application.Interfaces;

using WarehouseManagement.Application.DTOs;

public interface IShipmentDocumentService
{
    Task<IEnumerable<ShipmentDocumentDto>> GetAllAsync(ShipmentFilterDto? filter = null);
    Task<ShipmentDocumentDto?> GetByIdAsync(int id);
    Task<ShipmentDocumentDto> CreateAsync(CreateShipmentDocumentDto dto);
    Task<ShipmentDocumentDto> UpdateAsync(int id, UpdateShipmentDocumentDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ShipmentDocumentDto> SignAsync(int id);
    Task<ShipmentDocumentDto> CancelAsync(int id);
}