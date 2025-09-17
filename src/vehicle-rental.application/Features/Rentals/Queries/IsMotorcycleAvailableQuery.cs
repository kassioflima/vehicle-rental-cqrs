using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.application.Features.Rentals.Queries;

public record IsMotorcycleAvailableQuery : IQuery<bool>
{
    public Guid MotorcycleId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
