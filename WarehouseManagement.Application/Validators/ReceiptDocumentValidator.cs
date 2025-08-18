using FluentValidation;
using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Validators;

public class CreateReceiptDocumentValidator : AbstractValidator<CreateReceiptDocumentDto>
{
    public CreateReceiptDocumentValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters");
            
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Date cannot be in the future");
            
        RuleForEach(x => x.Resources)
            .SetValidator(new CreateReceiptResourceValidator());
    }
}

public class UpdateReceiptDocumentValidator : AbstractValidator<UpdateReceiptDocumentDto>
{
    public UpdateReceiptDocumentValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters");
            
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Date cannot be in the future");
            
        RuleForEach(x => x.Resources)
            .SetValidator(new CreateReceiptResourceValidator());
    }
}

public class CreateReceiptResourceValidator : AbstractValidator<CreateReceiptResourceDto>
{
    public CreateReceiptResourceValidator()
    {
        RuleFor(x => x.ResourceId)
            .GreaterThan(0).WithMessage("Valid resource is required");
            
        RuleFor(x => x.UnitOfMeasurementId)
            .GreaterThan(0).WithMessage("Valid unit of measurement is required");
            
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(999999999).WithMessage("Quantity is too large");
    }
}