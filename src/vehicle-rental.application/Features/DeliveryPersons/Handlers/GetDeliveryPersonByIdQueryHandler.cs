using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.DeliveryPersons.Queries;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;

namespace vehicle_rental.application.Features.DeliveryPersons.Handlers;

public class GetDeliveryPersonByIdQueryHandler(
    IDeliveryPersonRepository deliveryPersonRepository,
    ILogger<GetDeliveryPersonByIdQueryHandler> logger) : IRequestHandler<GetDeliveryPersonByIdQuery, Result<DeliveryPersonDto?>>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<GetDeliveryPersonByIdQueryHandler> _logger = logger;

    public async Task<Result<DeliveryPersonDto?>> Handle(GetDeliveryPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(request.Id, cancellationToken);

        if (deliveryPerson == null)
        {
            return Result<DeliveryPersonDto?>.Success(null);
        }

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

        return Result<DeliveryPersonDto?>.Success(dto);
    }
}

public class GetDeliveryPersonsQueryHandler(
    IDeliveryPersonRepository deliveryPersonRepository,
    ILogger<GetDeliveryPersonsQueryHandler> logger) : IRequestHandler<GetDeliveryPersonsQuery, Result<IEnumerable<DeliveryPersonDto>>>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<GetDeliveryPersonsQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<DeliveryPersonDto>>> Handle(GetDeliveryPersonsQuery request, CancellationToken cancellationToken)
    {
        var deliveryPersons = await _deliveryPersonRepository.GetAllAsync(cancellationToken);

        var dtos = deliveryPersons.Select(dp => new DeliveryPersonDto
        {
            Id = dp.Id,
            Name = dp.Name!,
            Cnpj = dp.Cnpj!,
            BirthDate = dp.BirthDate,
            LicenseNumber = dp.LicenseNumber!,
            LicenseType = dp.LicenseType,
            LicenseImageUrl = dp.LicenseImageUrl,
            CreatedAt = dp.CreatedAt,
            UpdatedAt = dp.UpdatedAt
        });

        return Result<IEnumerable<DeliveryPersonDto>>.Success(dtos);
    }
}

public class CanRentMotorcycleQueryHandler(
    IDeliveryPersonRepository deliveryPersonRepository,
    ILogger<CanRentMotorcycleQueryHandler> logger) : IRequestHandler<CanRentMotorcycleQuery, Result<bool>>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly ILogger<CanRentMotorcycleQueryHandler> _logger = logger;

    public async Task<Result<bool>> Handle(CanRentMotorcycleQuery request, CancellationToken cancellationToken)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(request.DeliveryPersonId, cancellationToken);

        if (deliveryPerson == null)
        {
            return Result<bool>.Failure(
                ErrorCodes.DELIVERY_PERSON_NOT_FOUND, 
                ErrorMessages.DELIVERY_PERSON_NOT_FOUND);
        }

        var canRent = deliveryPerson.CanRentMotorcycle();
        return Result<bool>.Success(canRent);
    }
}
