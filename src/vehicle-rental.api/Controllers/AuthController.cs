using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_rental.api.Middleware;
using vehicle_rental.application.Features.Auth.Commands;
using vehicle_rental.domain.Domain.auth.dtos;

namespace vehicle_rental.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost("create-user")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserInfoDto>> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
