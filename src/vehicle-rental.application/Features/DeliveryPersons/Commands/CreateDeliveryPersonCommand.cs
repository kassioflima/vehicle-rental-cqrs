using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;

namespace vehicle_rental.application.Features.DeliveryPersons.Commands;

public record CreateDeliveryPersonCommand : ICommand<DeliveryPersonDto>
{
    public string Name { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public vehicle_rental.domain.Domain.deliveryPersons.enums.ELicenseType LicenseType { get; init; }
}

public record UpdateDeliveryPersonLicenseImageCommand : ICommand<DeliveryPersonDto>
{
    public Guid Id { get; init; }
    public Microsoft.AspNetCore.Http.IFormFile LicenseImage { get; init; } = null!;
}
