using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsOfMeasurementController : ControllerBase
{
    private readonly IUnitOfMeasurementService _unitService;
    
    public UnitsOfMeasurementController(IUnitOfMeasurementService unitService)
    {
        _unitService = unitService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitOfMeasurementDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var units = await _unitService.GetAllAsync(includeArchived);
        return Ok(units);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UnitOfMeasurementDto>> GetById(int id)
    {
        var unit = await _unitService.GetByIdAsync(id);
        if (unit == null)
            return NotFound();
            
        return Ok(unit);
    }
    
    [HttpPost]
    public async Task<ActionResult<UnitOfMeasurementDto>> Create(CreateUnitOfMeasurementDto dto)
    {
        try
        {
            var unit = await _unitService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<UnitOfMeasurementDto>> Update(int id, UpdateUnitOfMeasurementDto dto)
    {
        try
        {
            var unit = await _unitService.UpdateAsync(id, dto);
            return Ok(unit);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
    
    [HttpPost("{id}/archive")]
    public async Task<ActionResult> Archive(int id)
    {
        try
        {
            await _unitService.ArchiveAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _unitService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (EntityInUseException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}