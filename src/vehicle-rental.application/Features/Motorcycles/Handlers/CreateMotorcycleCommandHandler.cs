using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Motorcycles.Commands;
using vehicle_rental.domain.Domain.motorcycles.dtos;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.shared.interfaces;

namespace vehicle_rental.application.Features.Motorcycles.Handlers;

public class CreateMotorcycleCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    IMessagePublisher messagePublisher,
    ILogger<CreateMotorcycleCommandHandler> logger) : IRequestHandler<CreateMotorcycleCommand, Result<MotorcycleDto>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<CreateMotorcycleCommandHandler> _logger = logger;

    public async Task<Result<MotorcycleDto>> Handle(CreateMotorcycleCommand request, CancellationToken cancellationToken)
    {
        // Check if license plate already exists
        var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateExactAsync(request.LicensePlate, cancellationToken);

        if (existingMotorcycle != null)
        {
            return Result<MotorcycleDto>.Failure(
                ErrorCodes.MOTORCYCLE_LICENSE_PLATE_EXISTS, 
                ErrorMessages.MOTORCYCLE_LICENSE_PLATE_EXISTS);
        }

        var motorcycle = new Motorcycle(request.Year, request.Model, request.LicensePlate);

        await _motorcycleRepository.AddAsync(motorcycle, cancellationToken);

        _logger.LogInformation("Created motorcycle {MotorcycleId} with license plate {LicensePlate}",
            motorcycle.Id, motorcycle.LicensePlate);

        // Publish motorcycle registered event
        await _messagePublisher.PublishMotorcycleRegisteredAsync(
            motorcycle.Id, motorcycle.Year!, motorcycle.Model!, motorcycle.LicensePlate!);

        var dto = new MotorcycleDto
        {
            Id = motorcycle.Id,
            Year = motorcycle.Year!,
            Model = motorcycle.Model!,
            LicensePlate = motorcycle.LicensePlate!,
            CreatedAt = motorcycle.CreatedAt,
            UpdatedAt = motorcycle.UpdatedAt
        };

        return Result<MotorcycleDto>.Success(dto);
    }
}

public class UpdateMotorcycleLicensePlateCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<UpdateMotorcycleLicensePlateCommandHandler> logger) : IRequestHandler<UpdateMotorcycleLicensePlateCommand, Result<MotorcycleDto>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<UpdateMotorcycleLicensePlateCommandHandler> _logger = logger;

    public async Task<Result<MotorcycleDto>> Handle(UpdateMotorcycleLicensePlateCommand request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (motorcycle == null)
        {
            return Result<MotorcycleDto>.Failure(
                ErrorCodes.MOTORCYCLE_NOT_FOUND, 
                ErrorMessages.MOTORCYCLE_NOT_FOUND);
        }

        // Check if new license plate already exists
        var existingMotorcycle = await _motorcycleRepository.GetByLicensePlateExactAsync(request.NewLicensePlate, cancellationToken);
        if (existingMotorcycle != null && existingMotorcycle.Id != request.Id)
        {
            return Result<MotorcycleDto>.Failure(
                ErrorCodes.MOTORCYCLE_LICENSE_PLATE_EXISTS, 
                ErrorMessages.MOTORCYCLE_LICENSE_PLATE_EXISTS);
        }

        motorcycle.UpdateLicensePlate(request.NewLicensePlate);
        await _motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);

        _logger.LogInformation("Updated motorcycle {MotorcycleId} license plate to {NewLicensePlate}",
            motorcycle.Id, request.NewLicensePlate);

        var dto = new MotorcycleDto
        {
            Id = motorcycle.Id,
            Year = motorcycle.Year!,
            Model = motorcycle.Model!,
            LicensePlate = motorcycle.LicensePlate!,
            CreatedAt = motorcycle.CreatedAt,
            UpdatedAt = motorcycle.UpdatedAt
        };

        return Result<MotorcycleDto>.Success(dto);
    }
}

public class DeleteMotorcycleCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<DeleteMotorcycleCommandHandler> logger) : IRequestHandler<DeleteMotorcycleCommand, Result<bool>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<DeleteMotorcycleCommandHandler> _logger = logger;

    public async Task<Result<bool>> Handle(DeleteMotorcycleCommand request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (motorcycle == null)
        {
            return Result<bool>.Success(false);
        }

        // Check if motorcycle has active rentals
        var hasActiveRentals = await _motorcycleRepository.HasActiveRentalsAsync(request.Id, cancellationToken);
        if (hasActiveRentals)
        {
            return Result<bool>.Failure(
                ErrorCodes.MOTORCYCLE_HAS_ACTIVE_RENTALS, 
                ErrorMessages.MOTORCYCLE_HAS_ACTIVE_RENTALS);
        }

        var deleted = await _motorcycleRepository.DeleteAsync(request.Id, cancellationToken);

        if (deleted)
        {
            _logger.LogInformation("Deleted motorcycle {MotorcycleId}", request.Id);
        }

        return Result<bool>.Success(deleted);
    }
}
