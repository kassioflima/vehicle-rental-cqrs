using Microsoft.AspNetCore.Http;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.enums;

namespace vehicle_rental.domain.Domain.deliveryPersons.interfaces;

public interface IDeliveryPersonService
{
    Task<DeliveryPersonDto> CreateDeliveryPersonAsync(CreateDeliveryPersonDto createDto, CancellationToken cancellationToken = default);
    Task<DeliveryPersonDto?> GetDeliveryPersonByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeliveryPersonDto>> GetDeliveryPersonsAsync(CancellationToken cancellationToken = default);
    Task<DeliveryPersonDto> UpdateLicenseImageAsync(Guid id, IFormFile licenseImage, CancellationToken cancellationToken = default);
    Task<bool> IsLicenseTypeValidAsync(ELicenseType licenseType, CancellationToken cancellationToken = default);
    Task<bool> CanRentMotorcycleAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
}
