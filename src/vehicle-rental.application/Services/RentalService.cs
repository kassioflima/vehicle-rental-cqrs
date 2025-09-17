using Microsoft.Extensions.Logging;
using vehicle_rental.domain.Domain.deliveryPersons.interfaces;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.entity;
using vehicle_rental.domain.Domain.rentals.enums;
using vehicle_rental.domain.Domain.rentals.interfaces;
using vehicle_rental.domain.Domain.rentals.mapper;

namespace vehicle_rental.application.Services;

public class RentalService(
    IRentalRepository rentalRepository,
    IDeliveryPersonRepository deliveryPersonRepository,
    IMotorcycleRepository motorcycleRepository,
    ILogger<RentalService> logger) : IRentalService
{
    private readonly IRentalRepository _rentalRepository = rentalRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository = deliveryPersonRepository;
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<RentalService> _logger = logger;

    public async Task<RentalDto?> CreateRentalAsync(CreateRentalDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);
        cancellationToken.ThrowIfCancellationRequested();

        // Validate delivery person exists and can rent
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(createDto.DeliveryPersonId, cancellationToken);
        if (deliveryPerson == null)
        {
            throw new InvalidOperationException($"Delivery person with id {createDto.DeliveryPersonId} not found");
        }

        if (!deliveryPerson.CanRentMotorcycle())
        {
            throw new InvalidOperationException($"Delivery person {createDto.DeliveryPersonId} cannot rent motorcycles (license type: {deliveryPerson.LicenseType})");
        }

        // Validate motorcycle exists
        var motorcycle = await _motorcycleRepository.GetByIdAsync(createDto.MotorcycleId, cancellationToken);
        if (motorcycle == null)
        {
            throw new InvalidOperationException($"Motorcycle with id {createDto.MotorcycleId} not found");
        }

        // Check if motorcycle is available
        var startDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc); // Start date is tomorrow
        var endDate = DateTime.SpecifyKind(startDate.AddDays((int)createDto.Plan), DateTimeKind.Utc);
        var expectedEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        if (!await _rentalRepository.IsMotorcycleAvailableAsync(createDto.MotorcycleId, startDate, endDate, cancellationToken))
        {
            throw new InvalidOperationException($"Motorcycle {createDto.MotorcycleId} is not available for the selected period");
        }

        var dailyRate = await GetDailyRateAsync(createDto.Plan, cancellationToken);
        var rental = new Rental(
            createDto.DeliveryPersonId,
            createDto.MotorcycleId,
            createDto.Plan,
            startDate,
            endDate,
            expectedEndDate,
            dailyRate
        );

        await _rentalRepository.AddAsync(rental, cancellationToken);

        _logger.LogInformation("Created rental {RentalId} for delivery person {DeliveryPersonId} and motorcycle {MotorcycleId}",
            rental.Id, createDto.DeliveryPersonId, createDto.MotorcycleId);

        return rental.MapToDto();
    }

    public async Task<RentalDto?> GetRentalByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        return rental?.MapToDto();
    }

    public async Task<IEnumerable<RentalDto>> GetRentalsByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rentals = await _rentalRepository.GetByDeliveryPersonIdAsync(deliveryPersonId, cancellationToken);
        return rentals.MapToDto();
    }

    public async Task<RentalCalculationDto> CalculateRentalReturnAsync(Guid rentalId, DateTime returnDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            throw new InvalidOperationException($"Rental with id {rentalId} not found");
        }

        return rental.CalculateReturnAmount(returnDate);
    }

    public async Task<RentalDto?> CompleteRentalAsync(Guid rentalId, DateTime returnDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);
        if (rental == null)
        {
            throw new InvalidOperationException($"Rental with id {rentalId} not found");
        }

        if (rental.Status != RentalStatus.Active)
        {
            throw new InvalidOperationException($"Rental {rentalId} is not active");
        }

        rental.CompleteRental(returnDate);
        await _rentalRepository.UpdateAsync(rental, cancellationToken);

        _logger.LogInformation("Completed rental {RentalId} with return date {ReturnDate}", rentalId, returnDate);
        return rental.MapToDto();
    }

    public async Task<decimal> GetDailyRateAsync(ERentalPlan plan, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.FromResult(plan switch
        {
            ERentalPlan.SevenDays => 30.00m,
            ERentalPlan.FifteenDays => 28.00m,
            ERentalPlan.ThirtyDays => 22.00m,
            ERentalPlan.FortyFiveDays => 20.00m,
            ERentalPlan.FiftyDays => 18.00m,
            _ => throw new ArgumentException($"Invalid rental plan: {plan}")
        });
    }

    public async Task<bool> IsMotorcycleAvailableAsync(Guid motorcycleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _rentalRepository.IsMotorcycleAvailableAsync(motorcycleId, startDate, endDate, cancellationToken);
    }
}
