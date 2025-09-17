using Microsoft.Extensions.Logging;
using vehicle_rental.domain.Domain.motorcycles.dtos;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.motorcycles.mapper;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.application.Services;

public class MotorcycleService(
    IMotorcycleRepository motorcycleRepository,
    ILogger<MotorcycleService> logger,
    IMessagePublisher messagePublisher) : IMotorcycleService
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<MotorcycleService> _logger = logger;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;

    public async Task<MotorcycleDto?> CreateMotorcycleAsync(CreateMotorcycleDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);
        cancellationToken.ThrowIfCancellationRequested();

        // Check if license plate already exists
        var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateExactAsync(createDto.LicensePlate, cancellationToken);

        if (existingMotorcycle != null)
        {
            throw new InvalidOperationException($"Motorcycle with license plate {createDto.LicensePlate} already exists");
        }

        var motorcycle = new Motorcycle(createDto.Year, createDto.Model, createDto.LicensePlate);

        await _motorcycleRepository.AddAsync(motorcycle, cancellationToken);

        _logger.LogInformation("Created motorcycle {MotorcycleId} with license plate {LicensePlate}",
            motorcycle.Id, motorcycle.LicensePlate);

        // Publish motorcycle registered event
        await _messagePublisher.PublishMotorcycleRegisteredAsync(
            motorcycle.Id, motorcycle.Year!, motorcycle.Model!, motorcycle.LicensePlate!);

        return motorcycle.MapToDto();
    }

    public async Task<MotorcycleDto?> GetMotorcycleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        return motorcycle?.MapToDto();
    }

    public async Task<IEnumerable<MotorcycleDto>> GetMotorcyclesAsync(string? licensePlate = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<Motorcycle> motorcycles;

        if (!string.IsNullOrEmpty(licensePlate))
        {
            motorcycles = await _motorcycleRepository.GetByLicensePlateAsync(licensePlate, cancellationToken);
        }
        else
        {
            motorcycles = await _motorcycleRepository.GetAllAsync(cancellationToken);
        }

        return motorcycles.MapToDto();
    }

    public async Task<MotorcycleDto?> UpdateMotorcycleLicensePlateAsync(Guid id, string newLicensePlate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newLicensePlate);
        cancellationToken.ThrowIfCancellationRequested();

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken) ?? 
            throw new InvalidOperationException($"Motorcycle with id {id} not found");

        // Check if new license plate already exists
        var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateExactAsync(newLicensePlate, cancellationToken);
        if (existingMotorcycle != null && existingMotorcycle.Id != id)
        {
            throw new InvalidOperationException($"Motorcycle with license plate {newLicensePlate} already exists");
        }

        motorcycle.UpdateLicensePlate(newLicensePlate);
        await _motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);

        _logger.LogInformation("Updated license plate for motorcycle {MotorcycleId} to {NewLicensePlate}",
            motorcycle.Id, newLicensePlate);

        return motorcycle.MapToDto();
    }

    public async Task<bool> DeleteMotorcycleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle == null)
        {
            return false;
        }

        // Check if motorcycle has active rentals
        var hasActiveRentals = await _motorcycleRepository.HasActiveRentalsAsync(id, cancellationToken);
        if (hasActiveRentals)
        {
            throw new InvalidOperationException($"Cannot delete motorcycle {id} because it has active rentals");
        }

        var result = await _motorcycleRepository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("Deleted motorcycle {MotorcycleId}", id);
        return result;
    }

    public async Task<bool> HasActiveRentalsAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _motorcycleRepository.HasActiveRentalsAsync(motorcycleId, cancellationToken);
    }
}
