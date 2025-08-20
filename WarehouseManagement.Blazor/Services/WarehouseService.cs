namespace WarehouseManagement.Blazor.Services;

using WarehouseManagement.Blazor.Models;

public class WarehouseService : IWarehouseService
{
    private readonly IApiClient _apiClient;
    
    public WarehouseService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    
    public async Task<List<ResourceDto>> GetResourcesAsync(bool includeArchived = false)
    {
        var result = await _apiClient.GetAsync<List<ResourceDto>>($"api/resources?includeArchived={includeArchived}");
        return result ?? new List<ResourceDto>();
    }
    
    public async Task<ResourceDto?> GetResourceAsync(int id)
    {
        return await _apiClient.GetAsync<ResourceDto>($"api/resources/{id}");
    }
    
    public async Task<ResourceDto?> CreateResourceAsync(CreateResourceDto dto)
    {
        return await _apiClient.PostAsync<ResourceDto>("api/resources", dto);
    }
    
    public async Task<ResourceDto?> UpdateResourceAsync(int id, UpdateResourceDto dto)
    {
        return await _apiClient.PutAsync<ResourceDto>($"api/resources/{id}", dto);
    }
    
    public async Task<bool> ArchiveResourceAsync(int id)
    {
        var response = await _apiClient.PostAsync<bool>($"api/resources/{id}/archive", new { });
        return response;
    }
    
    public async Task<bool> DeleteResourceAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/resources/{id}");
    }
    
    public async Task<List<UnitOfMeasurementDto>> GetUnitsAsync(bool includeArchived = false)
    {
        var result = await _apiClient.GetAsync<List<UnitOfMeasurementDto>>($"api/unitsofmeasurement?includeArchived={includeArchived}");
        return result ?? new List<UnitOfMeasurementDto>();
    }
    
    public async Task<UnitOfMeasurementDto?> GetUnitAsync(int id)
    {
        return await _apiClient.GetAsync<UnitOfMeasurementDto>($"api/unitsofmeasurement/{id}");
    }
    
    public async Task<UnitOfMeasurementDto?> CreateUnitAsync(CreateUnitOfMeasurementDto dto)
    {
        return await _apiClient.PostAsync<UnitOfMeasurementDto>("api/unitsofmeasurement", dto);
    }
    
    public async Task<UnitOfMeasurementDto?> UpdateUnitAsync(int id, UpdateUnitOfMeasurementDto dto)
    {
        return await _apiClient.PutAsync<UnitOfMeasurementDto>($"api/unitsofmeasurement/{id}", dto);
    }
    
    public async Task<bool> ArchiveUnitAsync(int id)
    {
        var response = await _apiClient.PostAsync<bool>($"api/unitsofmeasurement/{id}/archive", new { });
        return response;
    }
    
    public async Task<bool> DeleteUnitAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/unitsofmeasurement/{id}");
    }
    
    public async Task<List<ClientDto>> GetClientsAsync(bool includeArchived = false)
    {
        var result = await _apiClient.GetAsync<List<ClientDto>>($"api/clients?includeArchived={includeArchived}");
        return result ?? new List<ClientDto>();
    }
    
    public async Task<ClientDto?> GetClientAsync(int id)
    {
        return await _apiClient.GetAsync<ClientDto>($"api/clients/{id}");
    }
    
    public async Task<ClientDto?> CreateClientAsync(CreateClientDto dto)
    {
        return await _apiClient.PostAsync<ClientDto>("api/clients", dto);
    }
    
    public async Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto dto)
    {
        return await _apiClient.PutAsync<ClientDto>($"api/clients/{id}", dto);
    }
    
    public async Task<bool> ArchiveClientAsync(int id)
    {
        var response = await _apiClient.PostAsync<bool>($"api/clients/{id}/archive", new { });
        return response;
    }
    
    public async Task<bool> DeleteClientAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/clients/{id}");
    }
    
    public async Task<List<WarehouseStockDto>> GetStockAsync(WarehouseStockFilterDto? filter = null)
    {
        var query = filter != null ? BuildQueryString(filter) : "";
        var result = await _apiClient.GetAsync<List<WarehouseStockDto>>($"api/warehouse/stock{query}");
        return result ?? new List<WarehouseStockDto>();
    }
    
    public async Task<PagedResult<WarehouseStockDto>> GetPagedStockAsync(WarehouseStockFilterDto filter)
    {
        var query = BuildQueryString(filter);
        var result = await _apiClient.GetAsync<PagedResult<WarehouseStockDto>>($"api/reports/warehouse-stock{query}");
        return result ?? new PagedResult<WarehouseStockDto>();
    }
    
    public async Task<List<ReceiptDocumentDto>> GetReceiptsAsync(ReceiptFilterDto? filter = null)
    {
        var query = filter != null ? BuildQueryString(filter) : "";
        var result = await _apiClient.GetAsync<List<ReceiptDocumentDto>>($"api/receiptdocuments{query}");
        return result ?? new List<ReceiptDocumentDto>();
    }
    
    public async Task<ReceiptDocumentDto?> GetReceiptAsync(int id)
    {
        return await _apiClient.GetAsync<ReceiptDocumentDto>($"api/receiptdocuments/{id}");
    }
    
    public async Task<ReceiptDocumentDto?> CreateReceiptAsync(CreateReceiptDocumentDto dto)
    {
        return await _apiClient.PostAsync<ReceiptDocumentDto>("api/receiptdocuments", dto);
    }
    
    public async Task<ReceiptDocumentDto?> UpdateReceiptAsync(int id, UpdateReceiptDocumentDto dto)
    {
        return await _apiClient.PutAsync<ReceiptDocumentDto>($"api/receiptdocuments/{id}", dto);
    }
    
    public async Task<bool> DeleteReceiptAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/receiptdocuments/{id}");
    }
    
    public async Task<List<ShipmentDocumentDto>> GetShipmentsAsync(ShipmentFilterDto? filter = null)
    {
        var query = filter != null ? BuildQueryString(filter) : "";
        var result = await _apiClient.GetAsync<List<ShipmentDocumentDto>>($"api/shipmentdocuments{query}");
        return result ?? new List<ShipmentDocumentDto>();
    }
    
    public async Task<ShipmentDocumentDto?> GetShipmentAsync(int id)
    {
        return await _apiClient.GetAsync<ShipmentDocumentDto>($"api/shipmentdocuments/{id}");
    }
    
    public async Task<ShipmentDocumentDto?> CreateShipmentAsync(CreateShipmentDocumentDto dto)
    {
        return await _apiClient.PostAsync<ShipmentDocumentDto>("api/shipmentdocuments", dto);
    }
    
    public async Task<ShipmentDocumentDto?> UpdateShipmentAsync(int id, UpdateShipmentDocumentDto dto)
    {
        return await _apiClient.PutAsync<ShipmentDocumentDto>($"api/shipmentdocuments/{id}", dto);
    }
    
    public async Task<bool> DeleteShipmentAsync(int id)
    {
        return await _apiClient.DeleteAsync($"api/shipmentdocuments/{id}");
    }
    
    public async Task<ShipmentDocumentDto?> SignShipmentAsync(int id)
    {
        return await _apiClient.PostAsync<ShipmentDocumentDto>($"api/shipmentdocuments/{id}/sign", new { });
    }
    
    public async Task<ShipmentDocumentDto?> CancelShipmentAsync(int id)
    {
        return await _apiClient.PostAsync<ShipmentDocumentDto>($"api/shipmentdocuments/{id}/cancel", new { });
    }
    
    private string BuildQueryString(object filter)
    {
        var properties = filter.GetType().GetProperties();
        var queryParams = new List<string>();
        
        foreach (var prop in properties)
        {
            var value = prop.GetValue(filter);
            if (value != null)
            {
                if (value is IEnumerable<int> intList)
                {
                    foreach (var item in intList)
                        queryParams.Add($"{prop.Name}={item}");
                }
                else if (value is IEnumerable<string> stringList)
                {
                    foreach (var item in stringList)
                        queryParams.Add($"{prop.Name}={item}");
                }
                else
                {
                    queryParams.Add($"{prop.Name}={value}");
                }
            }
        }
        
        return queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
    }
}