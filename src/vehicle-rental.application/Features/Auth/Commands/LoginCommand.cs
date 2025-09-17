using vehicle_rental.application.Common.Interfaces;
using vehicle_rental.domain.Domain.auth.dtos;

namespace vehicle_rental.application.Features.Auth.Commands;

public record LoginCommand : ICommand<LoginResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
