using FluentValidation;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.enums;

namespace vehicle_rental.api.Validators;

public class CreateDeliveryPersonDtoValidator : AbstractValidator<CreateDeliveryPersonDto>
{
    public CreateDeliveryPersonDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome deve ter no máximo 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório")
            .Matches(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$")
            .WithMessage("CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Now.AddYears(-18))
            .WithMessage("Entregador deve ter pelo menos 18 anos")
            .GreaterThan(DateTime.Now.AddYears(-100))
            .WithMessage("Data de nascimento inválida");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .WithMessage("Número da CNH é obrigatório")
            .Matches(@"^\d{11}$")
            .WithMessage("Número da CNH deve conter exatamente 11 dígitos");

        RuleFor(x => x.LicenseType)
            .IsInEnum()
            .WithMessage("Tipo de CNH inválido")
            .Must(BeValidLicenseType)
            .WithMessage("Tipo de CNH deve ser A ou AB para alugar motocicletas");
    }

    private static bool BeValidLicenseType(ELicenseType licenseType)
    {
        return licenseType == ELicenseType.A || licenseType == ELicenseType.AB;
    }
}
