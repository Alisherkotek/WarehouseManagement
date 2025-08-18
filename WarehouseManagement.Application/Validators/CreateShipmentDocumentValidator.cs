using FluentValidation;
using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Validators;

public class CreateShipmentDocumentValidator : AbstractValidator<CreateShipmentDocumentDto>
{
    public CreateShipmentDocumentValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Date cannot be in the future");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Valid client is required");

        RuleFor(x => x.Resources)
            .NotEmpty().WithMessage("Shipment document cannot be empty")
            .Must(r => r != null && r.Any()).WithMessage("At least one resource is required");

        RuleForEach(x => x.Resources)
            .SetValidator(new CreateShipmentResourceValidator());
    }
}

public class UpdateShipmentDocumentValidator : AbstractValidator<UpdateShipmentDocumentDto>
{
    public UpdateShipmentDocumentValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("Date cannot be in the future");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Valid client is required");

        RuleFor(x => x.Resources)
            .NotEmpty().WithMessage("Shipment document cannot be empty")
            .Must(r => r != null && r.Any()).WithMessage("At least one resource is required");

        RuleForEach(x => x.Resources)
            .SetValidator(new CreateShipmentResourceValidator());
    }
}

public class CreateShipmentResourceValidator : AbstractValidator<CreateShipmentResourceDto>
{
    public CreateShipmentResourceValidator()
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