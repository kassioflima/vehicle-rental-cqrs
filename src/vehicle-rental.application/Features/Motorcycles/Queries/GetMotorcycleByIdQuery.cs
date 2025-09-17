using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.application.Features.Motorcycles.Queries;

public record GetMotorcycleByIdQuery : IQuery<MotorcycleDto?>
{
    public Guid Id { get; init; }
}