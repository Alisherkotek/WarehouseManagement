using FluentValidation;
using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Validators;

public class CreateUnitOfMeasurementValidator : AbstractValidator<CreateUnitOfMeasurementDto>
{
    public CreateUnitOfMeasurementValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Unit name is required")
            .MaximumLength(50).WithMessage("Unit name cannot exceed 50 characters");
    }
}

public class UpdateUnitOfMeasurementValidator : AbstractValidator<UpdateUnitOfMeasurementDto>
{
    public UpdateUnitOfMeasurementValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Unit name is required")
            .MaximumLength(50).WithMessage("Unit name cannot exceed 50 characters");
    }
}