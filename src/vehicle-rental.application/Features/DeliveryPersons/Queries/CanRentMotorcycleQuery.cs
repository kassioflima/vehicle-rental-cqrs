using vehicle_rental.application.Common.Interfaces;

namespace vehicle_rental.application.Features.DeliveryPersons.Queries;

public record CanRentMotorcycleQuery : IQuery<bool>
{
    public Guid DeliveryPersonId { get; init; }
}