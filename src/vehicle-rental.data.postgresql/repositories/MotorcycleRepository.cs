using Microsoft.EntityFrameworkCore;
using vehicle_rental.data.postgresql.context;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.rentals.enums;

namespace vehicle_rental.data.postgresql.repositories;

public class MotorcycleRepository(ApplicationDbContext context) : BaseRepository<Motorcycle>(context), IMotorcycleRepository
{
    public async Task<IEnumerable<Motorcycle>> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.LicensePlate!.Contains(licensePlate))
            .ToListAsync(cancellationToken);
    }

    public async Task<Motorcycle?> GetByLicensePlateExactAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate, cancellationToken);
    }

    public async Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _context.Rentals
            .AnyAsync(r => r.MotorcycleId == motorcycleId && r.Status == RentalStatus.Active, cancellationToken);
    }
}
