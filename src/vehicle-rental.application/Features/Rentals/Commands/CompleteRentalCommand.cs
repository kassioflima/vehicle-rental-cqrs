using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.application.Features.Rentals.Commands;

public record CompleteRentalCommand : ICommand<RentalDto>
{
    public Guid Id { get; init; }
    public DateTime ReturnDate { get; init; }
}