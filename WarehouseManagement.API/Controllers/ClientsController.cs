using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    
    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var clients = await _clientService.GetAllAsync(includeArchived);
        return Ok(clients);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);
        if (client == null)
            return NotFound();
            
        return Ok(client);
    }
    
    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientDto dto)
    {
        try
        {
            var client = await _clientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(int id, UpdateClientDto dto)
    {
        try
        {
            var client = await _clientService.UpdateAsync(id, dto);
            return Ok(client);
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
            await _clientService.ArchiveAsync(id);
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
            await _clientService.DeleteAsync(id);
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