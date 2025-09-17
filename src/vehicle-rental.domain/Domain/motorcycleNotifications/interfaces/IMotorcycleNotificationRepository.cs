using vehicle_rental.domain.Domain.motorcycleNotifications.entity;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.domain.Domain.motorcycleNotifications.interfaces;

public interface IMotorcycleNotificationRepository : IRepository<MotorcycleNotification>
{
    Task<IEnumerable<MotorcycleNotification>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleNotification>> GetByYearAsync(string year, CancellationToken cancellationToken = default);
}
