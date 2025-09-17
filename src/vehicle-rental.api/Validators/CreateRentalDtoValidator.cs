using FluentValidation;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.api.Validators;

public class CreateRentalDtoValidator : AbstractValidator<CreateRentalDto>
{
    public CreateRentalDtoValidator()
    {
        RuleFor(x => x.DeliveryPersonId)
            .NotEmpty()
            .WithMessage("ID do entregador é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID do entregador deve ser válido");

        RuleFor(x => x.MotorcycleId)
            .NotEmpty()
            .WithMessage("ID da motocicleta é obrigatório")
            .NotEqual(Guid.Empty)
            .WithMessage("ID da motocicleta deve ser válido");

        RuleFor(x => x.Plan)
            .IsInEnum()
            .WithMessage("Plano de locação inválido");
    }
}
