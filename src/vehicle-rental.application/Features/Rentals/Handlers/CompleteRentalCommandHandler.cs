using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Rentals.Commands;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.interfaces;
using vehicle_rental.domain.Domain.rentals.mapper;

namespace vehicle_rental.application.Features.Rentals.Handlers;

public class CompleteRentalCommandHandler(
    IRentalRepository _rentalRepository,
    ILogger<CompleteRentalCommandHandler> _logger) : IRequestHandler<CompleteRentalCommand, Result<RentalDto>>
{
    public async Task<Result<RentalDto>> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rental == null)
        {
            return Result<RentalDto>.Failure(
                ErrorCodes.RENTAL_NOT_FOUND, 
                ErrorMessages.RENTAL_NOT_FOUND);
        }

        rental.CompleteRental(request.ReturnDate);
        await _rentalRepository.UpdateAsync(rental, cancellationToken);
        _logger.LogInformation("Completed rental {RentalId} on {ReturnDate}", request.Id, request.ReturnDate);
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
