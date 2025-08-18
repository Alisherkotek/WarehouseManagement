using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentDocumentsController : ControllerBase
{
    private readonly IShipmentDocumentService _shipmentService;
    
    public ShipmentDocumentsController(IShipmentDocumentService shipmentService)
    {
        _shipmentService = shipmentService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShipmentDocumentDto>>> GetAll(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] List<string>? numbers = null,
        [FromQuery] List<int>? clientIds = null,
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null,
        [FromQuery] List<ShipmentStatus>? statuses = null)
    {
        var filter = new ShipmentFilterDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Numbers = numbers,
            ClientIds = clientIds,
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds,
            Statuses = statuses
        };
        
        var shipments = await _shipmentService.GetAllAsync(filter);
        return Ok(shipments);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ShipmentDocumentDto>> GetById(int id)
    {
        var shipment = await _shipmentService.GetByIdAsync(id);
        if (shipment == null)
            return NotFound();
            
        return Ok(shipment);
    }
    
    [HttpPost]
    public async Task<ActionResult<ShipmentDocumentDto>> Create(CreateShipmentDocumentDto dto)
    {
        try
        {
            var shipment = await _shipmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ShipmentDocumentDto>> Update(int id, UpdateShipmentDocumentDto dto)
    {
        try
        {
            var shipment = await _shipmentService.UpdateAsync(id, dto);
            return Ok(shipment);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _shipmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost("{id}/sign")]
    public async Task<ActionResult<ShipmentDocumentDto>> Sign(int id)
    {
        try
        {
            var shipment = await _shipmentService.SignAsync(id);
            return Ok(shipment);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (InsufficientStockException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ShipmentDocumentDto>> Cancel(int id)
    {
        try
        {
            var shipment = await _shipmentService.CancelAsync(id);
            return Ok(shipment);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}