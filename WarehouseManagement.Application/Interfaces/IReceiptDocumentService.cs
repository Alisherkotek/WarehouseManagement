using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Interfaces;

public interface IReceiptDocumentService
{
    Task<IEnumerable<ReceiptDocumentDto>> GetAllAsync(ReceiptFilterDto? filter = null);
    Task<ReceiptDocumentDto?> GetByIdAsync(int id);
    Task<ReceiptDocumentDto> CreateAsync(CreateReceiptDocumentDto dto);
    Task<ReceiptDocumentDto> UpdateAsync(int id, UpdateReceiptDocumentDto dto);
    Task<bool> DeleteAsync(int id);
}