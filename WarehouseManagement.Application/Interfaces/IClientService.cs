using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync(bool includeArchived = false);
    Task<ClientDto?> GetByIdAsync(int id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
    Task<ClientDto> UpdateAsync(int id, UpdateClientDto dto);
    Task<bool> ArchiveAsync(int id);
    Task<bool> DeleteAsync(int id);
}