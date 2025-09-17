using FluentValidation;
using vehicle_rental.domain.Domain.auth.dtos;

namespace vehicle_rental.application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email deve ter um formato válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("Senha deve ter pelo menos 6 caracteres");
    }
}
