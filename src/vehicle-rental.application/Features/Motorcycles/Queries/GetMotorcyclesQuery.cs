using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.application.Features.Motorcycles.Queries;

public record GetMotorcyclesQuery : IQuery<IEnumerable<MotorcycleDto>>
{
    public string? LicensePlate { get; init; }
}

public record HasActiveRentalsQuery : IQuery<bool>
{
    public Guid MotorcycleId { get; init; }
}