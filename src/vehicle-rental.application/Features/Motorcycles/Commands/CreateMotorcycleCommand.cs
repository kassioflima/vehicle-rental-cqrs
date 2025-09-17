using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.application.Features.Motorcycles.Commands;

public record CreateMotorcycleCommand : ICommand<MotorcycleDto>
{
    public string Year { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string LicensePlate { get; init; } = string.Empty;
}