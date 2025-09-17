using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Motorcycles.Queries;
using vehicle_rental.domain.Domain.motorcycles.dtos;
using vehicle_rental.domain.Domain.motorcycles.interfaces;
using vehicle_rental.domain.Domain.motorcycles.mapper;

namespace vehicle_rental.application.Features.Motorcycles.Handlers;

public class GetMotorcycleByIdQueryHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<GetMotorcycleByIdQueryHandler> logger) : IRequestHandler<GetMotorcycleByIdQuery, Result<MotorcycleDto?>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<GetMotorcycleByIdQueryHandler> _logger = logger;

    public async Task<Result<MotorcycleDto?>> Handle(GetMotorcycleByIdQuery request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);
        return Result<MotorcycleDto?>.Success(motorcycle?.MapToDto());
    }
}

public class GetMotorcyclesQueryHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<GetMotorcyclesQueryHandler> logger) : IRequestHandler<GetMotorcyclesQuery, Result<IEnumerable<MotorcycleDto>>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<GetMotorcyclesQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<MotorcycleDto>>> Handle(GetMotorcyclesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<vehicle_rental.domain.Domain.motorcycles.entity.Motorcycle> motorcycles;

        if (!string.IsNullOrEmpty(request.LicensePlate))
        {
            motorcycles = await _motorcycleRepository.GetByLicensePlateAsync(request.LicensePlate, cancellationToken);
        }
        else
        {
            motorcycles = await _motorcycleRepository.GetAllAsync(cancellationToken);
        }

        return Result<IEnumerable<MotorcycleDto>>.Success(motorcycles.MapToDto());
    }
}

public class HasActiveRentalsQueryHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<HasActiveRentalsQueryHandler> logger) : IRequestHandler<HasActiveRentalsQuery, Result<bool>>
{
    private readonly IMotorcycleRepository _motorcycleRepository = motorcycleRepository;
    private readonly ILogger<HasActiveRentalsQueryHandler> _logger = logger;

    public async Task<Result<bool>> Handle(HasActiveRentalsQuery request, CancellationToken cancellationToken)
    {
        var hasActiveRentals = await _motorcycleRepository.HasActiveRentalsAsync(request.MotorcycleId, cancellationToken);
        return Result<bool>.Success(hasActiveRentals);
    }
}
