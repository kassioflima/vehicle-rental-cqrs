using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.enums;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.application.Services;

public class DeliveryPersonService(IDeliveryPersonRepository deliveryPersonRepository, ILogger<DeliveryPersonService> logger) : IDeliveryPersonService
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<DeliveryPersonService> _logger = logger;

    public async Task<DeliveryPersonDto> CreateDeliveryPersonAsync(CreateDeliveryPersonDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);
        cancellationToken.ThrowIfCancellationRequested();

        // Check if CNPJ already exists
        var existingByCnpj = await _deliveryPersonRepository.CnpjExistsAsync(createDto.Cnpj, cancellationToken);
        if (existingByCnpj)
        {
            throw new InvalidOperationException($"Delivery person with CNPJ {createDto.Cnpj} already exists");
        }

        // Check if license number already exists
        var existingByLicense = await _deliveryPersonRepository.LicenseNumberExistsAsync(createDto.LicenseNumber, cancellationToken);
        if (existingByLicense)
        {
            throw new InvalidOperationException($"Delivery person with license number {createDto.LicenseNumber} already exists");
        }

        var deliveryPerson = new DeliveryPerson(
            createDto.Name,
            createDto.Cnpj,
            createDto.BirthDate,
            createDto.LicenseNumber,
            createDto.LicenseType
        );

        await _deliveryPersonRepository.AddAsync(deliveryPerson, cancellationToken);

        _logger.LogInformation("Created delivery person {DeliveryPersonId} with CNPJ {Cnpj}",
            deliveryPerson.Id, deliveryPerson.Cnpj);

        return MapToDto(deliveryPerson);
    }

    public async Task<DeliveryPersonDto?> GetDeliveryPersonByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        return deliveryPerson != null ? MapToDto(deliveryPerson) : null;
    }

    public async Task<IEnumerable<DeliveryPersonDto>> GetDeliveryPersonsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deliveryPersons = await _deliveryPersonRepository.GetAllAsync(cancellationToken);
        return deliveryPersons.Select(MapToDto);
    }

    public async Task<DeliveryPersonDto> UpdateLicenseImageAsync(Guid id, IFormFile licenseImage, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(licenseImage);
        cancellationToken.ThrowIfCancellationRequested();

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        if (deliveryPerson == null)
        {
            throw new InvalidOperationException($"Delivery person with id {id} not found");
        }

        // Validate file format
        var allowedExtensions = new[] { ".png", ".bmp" };
        var fileExtension = Path.GetExtension(licenseImage.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException($"File format {fileExtension} is not allowed. Only PNG and BMP files are accepted.");
        }

        // Generate unique filename
        var fileName = $"{deliveryPerson.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
        var filePath = Path.Combine("uploads", "licenses", fileName);

        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await licenseImage.CopyToAsync(stream, cancellationToken);
        }

        // Update delivery person with image URL
        deliveryPerson.UpdateLicenseImage($"/uploads/licenses/{fileName}");
        await _deliveryPersonRepository.UpdateAsync(deliveryPerson, cancellationToken);

        _logger.LogInformation("Updated license image for delivery person {DeliveryPersonId}", id);
        return MapToDto(deliveryPerson);
    }

    public async Task<bool> IsLicenseTypeValidAsync(ELicenseType licenseType, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.FromResult(Enum.IsDefined(typeof(ELicenseType), licenseType));
    }

    public async Task<bool> CanRentMotorcycleAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(deliveryPersonId, cancellationToken);
        if (deliveryPerson == null)
        {
            return false;
        }

        return deliveryPerson.CanRentMotorcycle();
    }

    private static DeliveryPersonDto MapToDto(DeliveryPerson deliveryPerson)
    {
        return new DeliveryPersonDto
        {
            Id = deliveryPerson.Id,
            Name = deliveryPerson.Name ?? string.Empty,
            Cnpj = deliveryPerson.Cnpj ?? string.Empty,
            BirthDate = deliveryPerson.BirthDate,
            LicenseNumber = deliveryPerson.LicenseNumber ?? string.Empty,
            LicenseType = deliveryPerson.LicenseType,
            LicenseImageUrl = deliveryPerson.LicenseImageUrl,
            CreatedAt = deliveryPerson.CreatedAt,
            UpdatedAt = deliveryPerson.UpdatedAt
        };
    }
}
