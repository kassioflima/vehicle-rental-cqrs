using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.DeliveryPersons.Commands;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.application.Features.DeliveryPersons.Handlers;

public class CreateDeliveryPersonCommandHandler(
    IDeliveryPersonRepository deliveryPersonRepository,
    ILogger<CreateDeliveryPersonCommandHandler> logger) : IRequestHandler<CreateDeliveryPersonCommand, Result<DeliveryPersonDto>>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<CreateDeliveryPersonCommandHandler> _logger = logger;

    public async Task<Result<DeliveryPersonDto>> Handle(CreateDeliveryPersonCommand request, CancellationToken cancellationToken)
    {
        // Check if CNPJ already exists
        var existingByCnpj = await _deliveryPersonRepository.CnpjExistsAsync(request.Cnpj, cancellationToken);
        if (existingByCnpj)
        {
            return Result<DeliveryPersonDto>.Failure(
                ErrorCodes.DELIVERY_PERSON_CNPJ_EXISTS, 
                ErrorMessages.DELIVERY_PERSON_CNPJ_EXISTS);
        }

        // Check if license number already exists
        var existingByLicense = await _deliveryPersonRepository.LicenseNumberExistsAsync(request.LicenseNumber, cancellationToken);
        if (existingByLicense)
        {
            return Result<DeliveryPersonDto>.Failure(
                ErrorCodes.DELIVERY_PERSON_LICENSE_EXISTS, 
                ErrorMessages.DELIVERY_PERSON_LICENSE_EXISTS);
        }

        var deliveryPerson = new DeliveryPerson(
            request.Name,
            request.Cnpj,
            request.BirthDate,
            request.LicenseNumber,
            request.LicenseType
        );

        await _deliveryPersonRepository.AddAsync(deliveryPerson, cancellationToken);

        _logger.LogInformation("Created delivery person {DeliveryPersonId} with CNPJ {Cnpj}",
            deliveryPerson.Id, deliveryPerson.Cnpj);

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
