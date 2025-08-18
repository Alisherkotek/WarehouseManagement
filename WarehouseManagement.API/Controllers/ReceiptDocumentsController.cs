using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptDocumentsController : ControllerBase
{
    private readonly IReceiptDocumentService _receiptService;
    
    public ReceiptDocumentsController(IReceiptDocumentService receiptService)
    {
        _receiptService = receiptService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceiptDocumentDto>>> GetAll(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] List<string>? numbers = null,
        [FromQuery] List<int>? resourceIds = null,
        [FromQuery] List<int>? unitIds = null)
    {
        var filter = new ReceiptFilterDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Numbers = numbers,
            ResourceIds = resourceIds,
            UnitOfMeasurementIds = unitIds
        };
        
        var receipts = await _receiptService.GetAllAsync(filter);
        return Ok(receipts);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ReceiptDocumentDto>> GetById(int id)
    {
        var receipt = await _receiptService.GetByIdAsync(id);
        if (receipt == null)
            return NotFound();
            
        return Ok(receipt);
    }
    
    [HttpPost]
    public async Task<ActionResult<ReceiptDocumentDto>> Create(CreateReceiptDocumentDto dto)
    {
        try
        {
            var receipt = await _receiptService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
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
    public async Task<ActionResult<ReceiptDocumentDto>> Update(int id, UpdateReceiptDocumentDto dto)
    {
        try
        {
            var receipt = await _receiptService.UpdateAsync(id, dto);
            return Ok(receipt);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { error = ex.Message });
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
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _receiptService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (InsufficientStockException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}