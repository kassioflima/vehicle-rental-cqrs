using Microsoft.EntityFrameworkCore;
using vehicle_rental.data.postgresql.context;
using vehicle_rental.domain.Domain.motorcycleNotifications.entity;
using vehicle_rental.domain.Domain.motorcycleNotifications.interfaces;

namespace vehicle_rental.data.postgresql.repositories;

public class MotorcycleNotificationRepository(ApplicationDbContext context) : BaseRepository<MotorcycleNotification>(context), IMotorcycleNotificationRepository
{
    public async Task<IEnumerable<MotorcycleNotification>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.MotorcycleId == motorcycleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotorcycleNotification>> GetByYearAsync(string year, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Year == year)
            .ToListAsync(cancellationToken);
    }
}
