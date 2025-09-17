using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.application.Features.Rentals.Queries;

public record GetRentalByIdQuery : IQuery<RentalDto?>
{
    public Guid Id { get; init; }
}
