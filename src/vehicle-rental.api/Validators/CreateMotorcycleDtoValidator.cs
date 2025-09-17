using FluentValidation;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.api.Validators;

public class CreateMotorcycleDtoValidator : AbstractValidator<CreateMotorcycleDto>
{
    public CreateMotorcycleDtoValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Ano é obrigatório")
            .Matches(@"^\d{4}$")
            .WithMessage("Ano deve conter exatamente 4 dígitos")
            .Must(BeValidYear)
            .WithMessage("Ano deve estar entre 1900 e o ano atual");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Modelo é obrigatório")
            .MaximumLength(50)
            .WithMessage("Modelo deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-Z0-9\s\-]+$")
            .WithMessage("Modelo deve conter apenas letras, números, espaços e hífens");

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .WithMessage("Placa é obrigatória")
            .Matches(@"^[A-Z]{3}\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$")
            .WithMessage("Placa deve estar no formato ABC1234 ou ABC1D23 (Mercosul)");
    }

    private static bool BeValidYear(string year)
    {
        if (!int.TryParse(year, out int yearValue))
            return false;

        return yearValue >= 1900 && yearValue <= DateTime.Now.Year;
    }
}
