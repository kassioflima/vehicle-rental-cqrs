using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.application.Features.Rentals.Commands;

public record CreateRentalCommand : ICommand<RentalDto>
{
    public Guid DeliveryPersonId { get; init; }
    public Guid MotorcycleId { get; init; }
    public ERentalPlan Plan { get; init; }
}
