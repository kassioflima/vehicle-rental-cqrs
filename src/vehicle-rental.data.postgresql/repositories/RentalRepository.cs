using Microsoft.EntityFrameworkCore;
using vehicle_rental.data.postgresql.context;
using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.domain.Domain.rentals.interfaces;

namespace vehicle_rental.data.postgresql.repositories;

public class RentalRepository(ApplicationDbContext context) : BaseRepository<Rental>(context), IRentalRepository
{
    public override async Task<Rental?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DeliveryPerson)
            .Include(r => r.Motorcycle)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DeliveryPerson)
            .Include(r => r.Motorcycle)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DeliveryPerson)
            .Include(r => r.Motorcycle)
            .Where(r => r.DeliveryPersonId == deliveryPersonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DeliveryPerson)
            .Include(r => r.Motorcycle)
            .Where(r => r.MotorcycleId == motorcycleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DeliveryPerson)
            .Include(r => r.Motorcycle)
            .Where(r => r.Status == RentalStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsMotorcycleAvailableAsync(Guid motorcycleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var conflictingRentals = await _dbSet
            .Where(r => r.MotorcycleId == motorcycleId &&
                       r.Status == RentalStatus.Active &&
                       ((r.StartDate <= startDate && r.ExpectedEndDate > startDate) ||
                        (r.StartDate < endDate && r.ExpectedEndDate >= endDate) ||
                        (r.StartDate >= startDate && r.ExpectedEndDate <= endDate)))
            .CountAsync(cancellationToken);

        return conflictingRentals == 0;
    }

    public async Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(r => r.MotorcycleId == motorcycleId && r.Status == RentalStatus.Active, cancellationToken);
    }

    public async Task<bool> HasActiveRentalsForDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(r => r.DeliveryPersonId == deliveryPersonId && r.Status == RentalStatus.Active, cancellationToken);
    }
}
