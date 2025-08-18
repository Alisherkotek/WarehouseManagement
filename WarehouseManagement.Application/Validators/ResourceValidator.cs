using FluentValidation;
using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Validators;

public class CreateResourceValidator : AbstractValidator<CreateResourceDto>
{
    public CreateResourceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(200).WithMessage("Resource name cannot exceed 200 characters");
    }
}

public class UpdateResourceValidator : AbstractValidator<UpdateResourceDto>
{
    public UpdateResourceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(200).WithMessage("Resource name cannot exceed 200 characters");
    }
}