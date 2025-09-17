using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.application.Features.Rentals.Queries;

public record CalculateRentalReturnQuery : IQuery<RentalCalculationDto>
{
    public Guid RentalId { get; init; }
    public DateTime ReturnDate { get; init; }
}
