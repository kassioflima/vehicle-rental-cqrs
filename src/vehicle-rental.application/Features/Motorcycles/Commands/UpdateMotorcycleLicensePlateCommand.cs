using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.application.Features.Motorcycles.Commands;

public record UpdateMotorcycleLicensePlateCommand : ICommand<MotorcycleDto>
{
    public Guid Id { get; init; }
    public string NewLicensePlate { get; init; } = string.Empty;
}