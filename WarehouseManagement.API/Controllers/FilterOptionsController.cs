using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterOptionsController : ControllerBase
{
    private readonly IResourceService _resourceService;
    private readonly IUnitOfMeasurementService _unitService;
    private readonly IClientService _clientService;
    private readonly IReceiptDocumentService _receiptService;
    private readonly IShipmentDocumentService _shipmentService;

    public FilterOptionsController(
        IResourceService resourceService,
        IUnitOfMeasurementService unitService,
        IClientService clientService,
        IReceiptDocumentService receiptService,
        IShipmentDocumentService shipmentService)
    {
        _resourceService = resourceService;
        _unitService = unitService;
        _clientService = clientService;
        _receiptService = receiptService;
        _shipmentService = shipmentService;
    }

    [HttpGet("resources")]
    public async Task<ActionResult<IEnumerable<object>>> GetResourceOptions([FromQuery] bool includeArchived = false)
    {
        var resources = await _resourceService.GetAllAsync(includeArchived);
        var options = resources.Select(r => new
        {
            value = r.Id,
            label = r.Name,
            isArchived = r.IsArchived
        });
        return Ok(options);
    }

    [HttpGet("units")]
    public async Task<ActionResult<IEnumerable<object>>> GetUnitOptions([FromQuery] bool includeArchived = false)
    {
        var units = await _unitService.GetAllAsync(includeArchived);
        var options = units.Select(u => new
        {
            value = u.Id,
            label = u.Name,
            isArchived = u.IsArchived
        });
        return Ok(options);
    }

    [HttpGet("clients")]
    public async Task<ActionResult<IEnumerable<object>>> GetClientOptions([FromQuery] bool includeArchived = false)
    {
        var clients = await _clientService.GetAllAsync(includeArchived);
        var options = clients.Select(c => new
        {
            value = c.Id,
            label = c.Name,
            isArchived = c.IsArchived
        });
        return Ok(options);
    }

    [HttpGet("receipt-numbers")]
    public async Task<ActionResult<IEnumerable<string>>> GetReceiptNumbers()
    {
        var receipts = await _receiptService.GetAllAsync();
        var numbers = receipts.Select(r => r.Number).Distinct().OrderBy(n => n);
        return Ok(numbers);
    }

    [HttpGet("shipment-numbers")]
    public async Task<ActionResult<IEnumerable<string>>> GetShipmentNumbers()
    {
        var shipments = await _shipmentService.GetAllAsync();
        var numbers = shipments.Select(s => s.Number).Distinct().OrderBy(n => n);
        return Ok(numbers);
    }

    [HttpGet("shipment-statuses")]
    public ActionResult<IEnumerable<object>> GetShipmentStatuses()
    {
        var statuses = Enum.GetValues<ShipmentStatus>()
            .Select(s => new
            {
                value = (int)s,
                label = s.ToString()
            });
        return Ok(statuses);
    }
}