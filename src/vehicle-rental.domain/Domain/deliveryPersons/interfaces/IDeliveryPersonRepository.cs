using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.domain.Domain.deliveryPersons.interfaces;

public interface IDeliveryPersonRepository : IRepository<DeliveryPerson>
{
    Task<DeliveryPerson?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<DeliveryPerson?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<bool> CnpjExistsAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken cancellationToken = default);
}
