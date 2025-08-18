using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Interfaces;

public interface IUnitOfMeasurementService
{
    Task<IEnumerable<UnitOfMeasurementDto>> GetAllAsync(bool includeArchived = false);
    Task<UnitOfMeasurementDto?> GetByIdAsync(int id);
    Task<UnitOfMeasurementDto> CreateAsync(CreateUnitOfMeasurementDto dto);
    Task<UnitOfMeasurementDto> UpdateAsync(int id, UpdateUnitOfMeasurementDto dto);
    Task<bool> ArchiveAsync(int id);
    Task<bool> DeleteAsync(int id);
}