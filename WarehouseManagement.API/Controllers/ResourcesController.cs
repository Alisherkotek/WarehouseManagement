using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;
    
    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var resources = await _resourceService.GetAllAsync(includeArchived);
        return Ok(resources);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ResourceDto>> GetById(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null)
            return NotFound();
            
        return Ok(resource);
    }
    
    [HttpPost]
    public async Task<ActionResult<ResourceDto>> Create(CreateResourceDto dto)
    {
        try
        {
            var resource = await _resourceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = resource.Id }, resource);
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ResourceDto>> Update(int id, UpdateResourceDto dto)
    {
        try
        {
            var resource = await _resourceService.UpdateAsync(id, dto);
            return Ok(resource);
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
            await _resourceService.ArchiveAsync(id);
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
            await _resourceService.DeleteAsync(id);
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