namespace WarehouseManagement.Blazor.Services;

using WarehouseManagement.Blazor.Models;

public interface IWarehouseService
{
    Task<List<ResourceDto>> GetResourcesAsync(bool includeArchived = false);
    Task<ResourceDto?> GetResourceAsync(int id);
    Task<ResourceDto?> CreateResourceAsync(CreateResourceDto dto);
    Task<ResourceDto?> UpdateResourceAsync(int id, UpdateResourceDto dto);
    Task<bool> ArchiveResourceAsync(int id);
    Task<bool> DeleteResourceAsync(int id);
    
    Task<List<UnitOfMeasurementDto>> GetUnitsAsync(bool includeArchived = false);
    Task<UnitOfMeasurementDto?> GetUnitAsync(int id);
    Task<UnitOfMeasurementDto?> CreateUnitAsync(CreateUnitOfMeasurementDto dto);
    Task<UnitOfMeasurementDto?> UpdateUnitAsync(int id, UpdateUnitOfMeasurementDto dto);
    Task<bool> ArchiveUnitAsync(int id);
    Task<bool> DeleteUnitAsync(int id);
    
    Task<List<ClientDto>> GetClientsAsync(bool includeArchived = false);
    Task<ClientDto?> GetClientAsync(int id);
    Task<ClientDto?> CreateClientAsync(CreateClientDto dto);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto dto);
    Task<bool> ArchiveClientAsync(int id);
    Task<bool> DeleteClientAsync(int id);
    
    Task<List<WarehouseStockDto>> GetStockAsync(WarehouseStockFilterDto? filter = null);
    Task<PagedResult<WarehouseStockDto>> GetPagedStockAsync(WarehouseStockFilterDto filter);
    
    Task<List<ReceiptDocumentDto>> GetReceiptsAsync(ReceiptFilterDto? filter = null);
    Task<ReceiptDocumentDto?> GetReceiptAsync(int id);
    Task<ReceiptDocumentDto?> CreateReceiptAsync(CreateReceiptDocumentDto dto);
    Task<ReceiptDocumentDto?> UpdateReceiptAsync(int id, UpdateReceiptDocumentDto dto);
    Task<bool> DeleteReceiptAsync(int id);
    
    Task<List<ShipmentDocumentDto>> GetShipmentsAsync(ShipmentFilterDto? filter = null);
    Task<ShipmentDocumentDto?> GetShipmentAsync(int id);
    Task<ShipmentDocumentDto?> CreateShipmentAsync(CreateShipmentDocumentDto dto);
    Task<ShipmentDocumentDto?> UpdateShipmentAsync(int id, UpdateShipmentDocumentDto dto);
    Task<bool> DeleteShipmentAsync(int id);
    Task<ShipmentDocumentDto?> SignShipmentAsync(int id);
    Task<ShipmentDocumentDto?> CancelShipmentAsync(int id);
}