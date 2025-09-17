using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using vehicle_rental.application.Common.Errors;
using vehicle_rental.application.Common.Models;
using vehicle_rental.application.Features.Auth.Commands;
using vehicle_rental.application.Services;
using vehicle_rental.application.Validators;
using vehicle_rental.domain.Domain.auth.dtos;
using vehicle_rental.domain.Domain.auth.entity;
using vehicle_rental.domain.Domain.auth.enums;

namespace vehicle_rental.application.Features.Auth.Handlers;

public class LoginCommandHandler(
    UserManager<User> _userManager,
    SignInManager<User> _signInManager,
    IJwtTokenService _jwtTokenService,
    ILogger<LoginCommandHandler> _logger) : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly LoginRequestValidator _validator = new();

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginRequest = new LoginRequestDto
        {
            Email = request.Email,
            Password = request.Password
        };

        var validation = await _validator.ValidateAsync(loginRequest, cancellationToken);

        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<LoginResponseDto>.Failure(
                ErrorCodes.AUTH_VALIDATION_ERROR,
                string.Join(", ", errors));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogInformation("Login. Houve uma tentativa de login para o email: {Email}, mas o usuário não foi encontrado na base de dados.", request.Email);
            return Result<LoginResponseDto>.Failure(
                ErrorCodes.AUTH_INVALID_CREDENTIALS,
                ErrorMessages.AUTH_INVALID_CREDENTIALS);
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("Login. Usuário bloqueado. Usuário: {Email}", user.Email);
            return Result<LoginResponseDto>.Failure(
                ErrorCodes.AUTH_USER_BLOCKED,
                "Usuário bloqueado. Procure o administrador do sistema para desbloqueio.");
        }

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        
        if (!loginResult.Succeeded)
        {
            _logger.LogInformation("Login. Usuário ou senha incorretos. Usuário: {Email}", user.Email);
            return Result<LoginResponseDto>.Failure(
                ErrorCodes.AUTH_INVALID_CREDENTIALS,
                ErrorMessages.AUTH_INVALID_CREDENTIALS);
        }

        if (loginResult.IsLockedOut)
        {
            _logger.LogInformation("Login. Usuário bloqueado por tentativas excessivas. Usuário: {Email}", user.Email);
            return Result<LoginResponseDto>.Failure(
                ErrorCodes.AUTH_USER_LOCKED_OUT,
                "Usuário bloqueado por tentativas excessivas. Tente novamente mais tarde.");
        }

        // Reset failed access count on successful login
        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = _jwtTokenService.GenerateToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Role = user.Role,
            DeliveryPersonId = user.DeliveryPersonId
        };

        var response = new LoginResponseDto
        {
            Token = jwtToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            User = userInfo
        };

        _logger.LogInformation("Login realizado com sucesso. Usuário: {Email}", user.Email);
        return Result<LoginResponseDto>.Success(response);
    }

    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}