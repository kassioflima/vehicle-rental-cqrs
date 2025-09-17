using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.domain.Domain.rentals.interfaces;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsMotorcycleAvailableAsync(Guid motorcycleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsForDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
}
