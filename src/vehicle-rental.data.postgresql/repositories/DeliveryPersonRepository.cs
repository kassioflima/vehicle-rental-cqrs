using Microsoft.EntityFrameworkCore;
using vehicle_rental.data.postgresql.context;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.data.postgresql.repositories;

public class DeliveryPersonRepository(ApplicationDbContext context) : BaseRepository<DeliveryPerson>(context), IDeliveryPersonRepository
{
    public async Task<DeliveryPerson?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.Cnpj == cnpj, cancellationToken);
    }

    public async Task<DeliveryPerson?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);
    }

    public async Task<bool> CnpjExistsAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(d => d.Cnpj == cnpj, cancellationToken);
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);
    }
}
