using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.DeliveryPersons.Commands;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.application.Features.DeliveryPersons.Handlers;

public class UpdateDeliveryPersonLicenseImageCommandHandler(
    IDeliveryPersonRepository deliveryPersonRepository,
    ILogger<UpdateDeliveryPersonLicenseImageCommandHandler> logger) : IRequestHandler<UpdateDeliveryPersonLicenseImageCommand, Result<DeliveryPersonDto>>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<UpdateDeliveryPersonLicenseImageCommandHandler> _logger = logger;

    public async Task<Result<DeliveryPersonDto>> Handle(UpdateDeliveryPersonLicenseImageCommand request, CancellationToken cancellationToken)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(request.Id, cancellationToken);

        if (deliveryPerson == null)
        {
            return Result<DeliveryPersonDto>.Failure(
                ErrorCodes.DELIVERY_PERSON_NOT_FOUND, 
                ErrorMessages.DELIVERY_PERSON_NOT_FOUND);
        }

        // Validate file format
        var allowedExtensions = new[] { ".png", ".bmp", ".jpg", ".jpeg" };
        var fileExtension = Path.GetExtension(request.LicenseImage.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<DeliveryPersonDto>.Failure(
                ErrorCodes.FILE_INVALID_FORMAT, 
                ErrorMessages.FILE_INVALID_FORMAT);
        }

        // Validate file size (5MB max)
        if (request.LicenseImage.Length > 5 * 1024 * 1024)
        {
            return Result<DeliveryPersonDto>.Failure(
                ErrorCodes.FILE_TOO_LARGE, 
                ErrorMessages.FILE_TOO_LARGE);
        }

        // In a real application, you would save the file and get the URL
        var imageUrl = $"uploads/licenses/{request.Id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

        deliveryPerson.UpdateLicenseImage(imageUrl);
        await _deliveryPersonRepository.UpdateAsync(deliveryPerson, cancellationToken);

        _logger.LogInformation("Updated license image for delivery person {DeliveryPersonId}", request.Id);

        var dto = new DeliveryPersonDto
        {
            Id = deliveryPerson.Id,
            Name = deliveryPerson.Name!,
            Cnpj = deliveryPerson.Cnpj!,
            BirthDate = deliveryPerson.BirthDate,
            LicenseNumber = deliveryPerson.LicenseNumber!,
            LicenseType = deliveryPerson.LicenseType,
            LicenseImageUrl = deliveryPerson.LicenseImageUrl,
            CreatedAt = deliveryPerson.CreatedAt,
            UpdatedAt = deliveryPerson.UpdatedAt
        };

        return Result<DeliveryPersonDto>.Success(dto);
    }
}
