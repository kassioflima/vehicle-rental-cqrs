using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.domain.Domain.motorcycles.interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<IEnumerable<Motorcycle>> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<Motorcycle?> GetByLicensePlateExactAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
}
