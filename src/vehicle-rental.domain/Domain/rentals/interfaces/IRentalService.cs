using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.domain.Domain.rentals.interfaces;

public interface IRentalService
{
    Task<RentalDto?> CreateRentalAsync(CreateRentalDto createDto, CancellationToken cancellationToken = default);
    Task<RentalDto?> GetRentalByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RentalDto>> GetRentalsByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<RentalCalculationDto> CalculateRentalReturnAsync(Guid rentalId, DateTime returnDate, CancellationToken cancellationToken = default);
    Task<RentalDto?> CompleteRentalAsync(Guid rentalId, DateTime returnDate, CancellationToken cancellationToken = default);
    Task<decimal> GetDailyRateAsync(ERentalPlan plan, CancellationToken cancellationToken = default);
    Task<bool> IsMotorcycleAvailableAsync(Guid motorcycleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
