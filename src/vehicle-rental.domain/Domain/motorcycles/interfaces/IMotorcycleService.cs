using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.domain.Domain.motorcycles.interfaces;

public interface IMotorcycleService
{
    Task<MotorcycleDto?> CreateMotorcycleAsync(CreateMotorcycleDto createDto, CancellationToken cancellationToken = default);
    Task<MotorcycleDto?> GetMotorcycleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleDto>> GetMotorcyclesAsync(string? licensePlate = null, CancellationToken cancellationToken = default);
    Task<MotorcycleDto?> UpdateMotorcycleLicensePlateAsync(Guid id, string newLicensePlate, CancellationToken cancellationToken = default);
    Task<bool> DeleteMotorcycleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
}
