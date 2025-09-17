using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Rentals.Queries;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.interfaces;
using vehicle_rental.domain.Domain.rentals.mapper;

namespace vehicle_rental.application.Features.Rentals.Handlers;

public class GetRentalByIdQueryHandler(
    IRentalRepository _rentalRepository,
    ILogger<GetRentalByIdQueryHandler> _logger) : IRequestHandler<GetRentalByIdQuery, Result<RentalDto?>>
{

    public async Task<Result<RentalDto?>> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id, cancellationToken);
        _logger.LogInformation("Fetched rental {RentalId}", request.Id);
        return Result<RentalDto?>.Success(rental?.MapToDto());
    }
}

public class GetRentalsByDeliveryPersonQueryHandler(
    IRentalRepository rentalRepository,
    ILogger<GetRentalsByDeliveryPersonQueryHandler> logger) : IRequestHandler<GetRentalsByDeliveryPersonQuery, Result<IEnumerable<RentalDto>>>
{
    private readonly IRentalRepository _rentalRepository = rentalRepository;
    private readonly ILogger<GetRentalsByDeliveryPersonQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<RentalDto>>> Handle(GetRentalsByDeliveryPersonQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetByDeliveryPersonIdAsync(request.DeliveryPersonId, cancellationToken);
        return Result<IEnumerable<RentalDto>>.Success(rentals.MapToDto());
    }
}

public class CalculateRentalReturnQueryHandler(
    IRentalRepository rentalRepository,
    ILogger<CalculateRentalReturnQueryHandler> logger) : IRequestHandler<CalculateRentalReturnQuery, Result<RentalCalculationDto>>
{
    private readonly IRentalRepository _rentalRepository = rentalRepository;
    private readonly ILogger<CalculateRentalReturnQueryHandler> _logger = logger;

    public async Task<Result<RentalCalculationDto>> Handle(CalculateRentalReturnQuery request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId, cancellationToken);
        
        if (rental == null)
        {
            return Result<RentalCalculationDto>.Failure(
                ErrorCodes.RENTAL_NOT_FOUND, 
                ErrorMessages.RENTAL_NOT_FOUND);
        }

        // Simple calculation logic - in real app this would be more complex
        var daysUsed = (request.ReturnDate - rental.StartDate).Days;
        var expectedDays = (rental.ExpectedEndDate - rental.StartDate).Days;
        var additionalDays = Math.Max(0, daysUsed - expectedDays);
        
        var additionalAmount = additionalDays * rental.DailyRate;
        var fineAmount = additionalDays > 0 ? additionalAmount * 0.2m : 0; // 20% fine for late return

        var calculation = new RentalCalculationDto
        {
            TotalAmount = rental.TotalAmount + additionalAmount + fineAmount,
            FineAmount = fineAmount,
            AdditionalDaysAmount = additionalAmount,
            DaysUsed = daysUsed,
            DaysRemaining = Math.Max(0, expectedDays - daysUsed),
            AdditionalDays = additionalDays
        };

        return Result<RentalCalculationDto>.Success(calculation);
    }
}

public class IsMotorcycleAvailableQueryHandler(
    IRentalRepository rentalRepository,
    ILogger<IsMotorcycleAvailableQueryHandler> logger) : IRequestHandler<IsMotorcycleAvailableQuery, Result<bool>>
{
    private readonly IRentalRepository _rentalRepository = rentalRepository;
    private readonly ILogger<IsMotorcycleAvailableQueryHandler> _logger = logger;

    public async Task<Result<bool>> Handle(IsMotorcycleAvailableQuery request, CancellationToken cancellationToken)
    {
        var isAvailable = await _rentalRepository.IsMotorcycleAvailableAsync(request.MotorcycleId, request.StartDate, request.EndDate, cancellationToken);
        return Result<bool>.Success(isAvailable);
    }
}
