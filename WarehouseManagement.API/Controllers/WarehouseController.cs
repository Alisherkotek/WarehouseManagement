using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    
    public WarehouseController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }
    
    [HttpGet("stock")]
    public async Task<ActionResult<IEnumerable<WarehouseStockDto>>> GetWarehouseStock(
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] bool includeZeroBalance = false)
    {
        var filter = new BalanceFilterDto
        {
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            IncludeZeroBalance = includeZeroBalance
        };
        
        var stock = await _balanceService.GetWarehouseStockAsync(filter);
        return Ok(stock);
    }
    
    [HttpGet("balances")]
    public async Task<ActionResult<IEnumerable<BalanceDto>>> GetBalances(
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] bool includeZeroBalance = false)
    {
        var filter = new BalanceFilterDto
        {
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            IncludeZeroBalance = includeZeroBalance
        };
        
        var balances = await _balanceService.GetAllBalancesAsync(filter);
        return Ok(balances);
    }
    
    [HttpGet("balance/{resourceId}/{unitId}")]
    public async Task<ActionResult<BalanceDto>> GetBalance(int resourceId, int unitId)
    {
        var balance = await _balanceService.GetBalanceAsync(resourceId, unitId);
        if (balance == null)
            return Ok(new BalanceDto 
            { 
                ResourceId = resourceId, 
                UnitOfMeasurementId = unitId, 
                Quantity = 0 
            });
            
        return Ok(balance);
    }
    
    [HttpGet("quantity/{resourceId}/{unitId}")]
    public async Task<ActionResult<decimal>> GetQuantity(int resourceId, int unitId)
    {
        var quantity = await _balanceService.GetQuantityAsync(resourceId, unitId);
        return Ok(quantity);
    }
    
    [HttpPost("check-availability")]
    public async Task<ActionResult<bool>> CheckAvailability([FromBody] CheckAvailabilityDto dto)
    {
        var hasQuantity = await _balanceService.HasSufficientQuantityAsync(
            dto.ResourceId, 
            dto.UnitOfMeasurementId, 
            dto.RequiredQuantity);
            
        return Ok(hasQuantity);
    }
}