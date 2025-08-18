using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Interfaces;

public interface IResourceService
{
    Task<IEnumerable<ResourceDto>> GetAllAsync(bool includeArchived = false);
    Task<ResourceDto?> GetByIdAsync(int id);
    Task<ResourceDto> CreateAsync(CreateResourceDto dto);
    Task<ResourceDto> UpdateAsync(int id, UpdateResourceDto dto);
    Task<bool> ArchiveAsync(int id);
    Task<bool> DeleteAsync(int id);
}