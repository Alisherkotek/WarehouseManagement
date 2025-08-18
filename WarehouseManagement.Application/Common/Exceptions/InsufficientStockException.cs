namespace WarehouseManagement.Application.Common.Exceptions;

public class InsufficientStockException : BusinessException
{
    public decimal RequiredQuantity { get; }
    public decimal AvailableQuantity { get; }
    
    public InsufficientStockException(string resourceName, decimal required, decimal available)
        : base($"Insufficient stock for {resourceName}. Required: {required}, Available: {available}", "INSUFFICIENT_STOCK")
    {
        RequiredQuantity = required;
        AvailableQuantity = available;
    }
}