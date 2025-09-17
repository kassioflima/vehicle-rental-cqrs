using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;

namespace vehicle_rental.application.Features.DeliveryPersons.Queries;

public record GetDeliveryPersonsQuery : IQuery<IEnumerable<DeliveryPersonDto>>
{
}