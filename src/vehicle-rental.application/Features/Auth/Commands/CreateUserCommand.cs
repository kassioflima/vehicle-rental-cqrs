using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.domain.Domain.auth.dtos;
using vehicle_rental.domain.Domain.auth.entity;
using vehicle_rental.domain.Domain.auth.enums;

namespace vehicle_rental.application.Features.Auth.Commands;

public record CreateUserCommand : IRequest<Result<UserInfoDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public EUserRole Role { get; set; }
    public Guid? DeliveryPersonId { get; set; }
}

public class CreateUserCommandHandler(
    UserManager<User> _userManager,
    ILogger<CreateUserCommandHandler> _logger) : IRequestHandler<CreateUserCommand, Result<UserInfoDto>>
{
    public async Task<Result<UserInfoDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogInformation("Tentativa de criar usuário com email já existente: {Email}", request.Email);
            return Result<UserInfoDto>.Failure(
                ErrorCodes.AUTH_USER_ALREADY_EXISTS,
                ErrorMessages.AUTH_USER_ALREADY_EXISTS);
        }

        // Create new user
        var user = new User(request.Email, request.Role, request.DeliveryPersonId);
        
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Falha ao criar usuário {Email}: {Errors}", request.Email, errors);
            return Result<UserInfoDto>.Failure(
                ErrorCodes.AUTH_VALIDATION_ERROR,
                errors);
        }

        // Add role
        await _userManager.AddToRoleAsync(user, request.Role.ToString());

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Role = user.Role,
            DeliveryPersonId = user.DeliveryPersonId
        };

        _logger.LogInformation("Usuário criado com sucesso: {Email}", request.Email);
        return Result<UserInfoDto>.Success(userInfo);
    }
}