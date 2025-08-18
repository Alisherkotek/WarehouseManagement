using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Interfaces;

public interface IBalanceService
{
    Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(BalanceFilterDto? filter = null);
    Task<BalanceDto?> GetBalanceAsync(int resourceId, int unitOfMeasurementId);
    Task<decimal> GetQuantityAsync(int resourceId, int unitOfMeasurementId);
    Task<bool> HasSufficientQuantityAsync(int resourceId, int unitOfMeasurementId, decimal requiredQuantity);
    Task<BalanceDto> AdjustBalanceAsync(int resourceId, int unitOfMeasurementId, decimal quantityChange, string reason);
    Task<IEnumerable<WarehouseStockDto>> GetWarehouseStockAsync(BalanceFilterDto? filter = null);
}