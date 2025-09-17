using FluentValidation;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.api.Validators;

public class UpdateMotorcycleLicensePlateDtoValidator : AbstractValidator<UpdateMotorcycleLicensePlateDto>
{
    public UpdateMotorcycleLicensePlateDtoValidator()
    {
        RuleFor(x => x.NewLicensePlate)
            .NotEmpty()
            .WithMessage("Nova placa é obrigatória")
            .Matches(@"^[A-Z]{3}\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$")
            .WithMessage("Placa deve estar no formato ABC1234 ou ABC1D23 (Mercosul)");
    }
}
