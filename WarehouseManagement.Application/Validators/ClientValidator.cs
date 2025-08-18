using FluentValidation;
using WarehouseManagement.Application.DTOs;

namespace WarehouseManagement.Application.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required")
            .MaximumLength(200).WithMessage("Client name cannot exceed 200 characters");
            
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Client address is required")
            .MaximumLength(500).WithMessage("Client address cannot exceed 500 characters");
    }
}

public class UpdateClientValidator : AbstractValidator<UpdateClientDto>
{
    public UpdateClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required")
            .MaximumLength(200).WithMessage("Client name cannot exceed 200 characters");
            
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Client address is required")
            .MaximumLength(500).WithMessage("Client address cannot exceed 500 characters");
    }
}