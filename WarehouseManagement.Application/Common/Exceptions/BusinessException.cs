namespace WarehouseManagement.Application.Common.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }
    
    public BusinessException(string message, string code = "BUSINESS_ERROR") 
        : base(message)
    {
        Code = code;
    }
}

public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string entityName, int id) 
        : base($"{entityName} with ID {id} not found", "NOT_FOUND")
    {
    }
}

public class DuplicateEntityException : BusinessException
{
    public DuplicateEntityException(string entityName, string fieldName, string value) 
        : base($"{entityName} with {fieldName} '{value}' already exists", "DUPLICATE")
    {
    }
}

public class EntityInUseException : BusinessException
{
    public EntityInUseException(string entityName) 
        : base($"Cannot delete {entityName} because it is in use", "IN_USE")
    {
    }
}