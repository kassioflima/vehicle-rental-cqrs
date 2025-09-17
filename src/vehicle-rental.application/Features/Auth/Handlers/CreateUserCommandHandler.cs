using MediatR;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Auth.Commands;
using vehicle_rental.domain.Domain.auth.dtos;

namespace vehicle_rental.application.Features.Auth.Handlers;

public class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand, Result<UserInfoDto>>
{
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public Task<Result<UserInfoDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // In a real application, you would save to database
        var user = new UserInfoDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Role = request.Role,
            DeliveryPersonId = request.DeliveryPersonId
        };

        _logger.LogInformation("Created user {Email} with role {Role}", request.Email, request.Role);

        return Task.FromResult(Result<UserInfoDto>.Success(user));
    }
}
