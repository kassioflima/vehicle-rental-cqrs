using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.application.Features.Rentals.Queries;

public record GetRentalsByDeliveryPersonQuery : IQuery<IEnumerable<RentalDto>>
{
    public Guid DeliveryPersonId { get; init; }
}