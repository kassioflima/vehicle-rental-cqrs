using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Rentals.Commands;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.interfaces;
using vehicle_rental.domain.Domain.rentals.mapper;

namespace vehicle_rental.application.Features.Rentals.Handlers;

public class CreateRentalCommandHandler(
    IRentalRepository _rentalRepository,
    IDeliveryPersonRepository _deliveryPersonRepository,
    IMotorcycleRepository _motorcycleRepository,
    ILogger<CreateRentalCommandHandler> _logger) : IRequestHandler<CreateRentalCommand, Result<RentalDto>>
{
    public async Task<Result<RentalDto>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(request.DeliveryPersonId, cancellationToken);
        if (deliveryPerson == null)
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.DELIVERY_PERSON_NOT_FOUND, 
                ErrorMessages.DELIVERY_PERSON_NOT_FOUND);
        }

        if (!deliveryPerson.CanRentMotorcycle())
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.DELIVERY_PERSON_CANNOT_RENT, 
                ErrorMessages.DELIVERY_PERSON_CANNOT_RENT);
        }

        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.MotorcycleId, cancellationToken);
        if (motorcycle == null)
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.MOTORCYCLE_NOT_FOUND, 
                ErrorMessages.MOTORCYCLE_NOT_FOUND);
        }

        var rental = new Rental(request.DeliveryPersonId, request.MotorcycleId, request.Plan);
        rental.SetStardDate(DateTime.Now);
        rental.SetEndDate(request.Plan);
        rental.SetDailyRate(request.Plan);
        rental.CalculateReturnAmount(rental.EndDate);

        var isAvailable = await _rentalRepository.IsMotorcycleAvailableAsync(request.MotorcycleId, DateTime.Now, rental.EndDate, cancellationToken);
        if (!isAvailable)
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.MOTORCYCLE_NOT_AVAILABLE, 
                ErrorMessages.MOTORCYCLE_NOT_AVAILABLE);
        }

        await _rentalRepository.AddAsync(rental, cancellationToken);

        _logger.LogInformation("Created rental {RentalId} for delivery person {DeliveryPersonId} and motorcycle {MotorcycleId}",
            rental.Id, request.DeliveryPersonId, request.MotorcycleId);

        var dto = rental.MapToDto();
        if (dto == null)
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.RENTAL_MAPPING_FAILED,
                ErrorMessages.RENTAL_MAPPING_FAILED);
        }

        return Result<RentalDto>.Success(dto);
    }
}
