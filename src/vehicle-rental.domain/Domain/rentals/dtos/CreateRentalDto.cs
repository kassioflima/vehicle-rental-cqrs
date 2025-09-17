using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.domain.Domain.rentals.dtos;

public record CreateRentalDto
{
    public Guid DeliveryPersonId { get; set; }
    public Guid MotorcycleId { get; set; }
    public ERentalPlan Plan { get; set; }
}
