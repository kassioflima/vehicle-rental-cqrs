using vehicle_rental.application.Common.Interfaces;

namespace vehicle_rental.application.Features.Motorcycles.Commands;

public record DeleteMotorcycleCommand : ICommand<bool>
{
    public Guid Id { get; init; }
}