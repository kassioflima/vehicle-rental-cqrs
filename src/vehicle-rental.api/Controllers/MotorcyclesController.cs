using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_rental.api.Middleware;
using vehicle_rental.application.Features.Motorcycles.Commands;
using vehicle_rental.application.Features.Motorcycles.Queries;
using vehicle_rental.domain.Domain.motorcycles.dtos;

namespace vehicle_rental.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MotorcyclesController(IMediator mediator, ILogger<MotorcyclesController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<MotorcyclesController> _logger = logger;

    /// <summary>
    /// Create a new motorcycle
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MotorcycleDto?>?> CreateMotorcycle([FromBody] CreateMotorcycleCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _mediator.Send(command, cancellationToken);
        return result?.ToActionResult();
    }

    /// <summary>
    /// Get motorcycle by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MotorcycleDto?>?> GetMotorcycle(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetMotorcycleByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }
        
        return result?.ToActionResult();
    }

    /// <summary>
    /// Get all motorcycles with optional license plate filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MotorcycleDto>?>?> GetMotorcycles([FromQuery] string? licensePlate = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetMotorcyclesQuery { LicensePlate = licensePlate };
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update motorcycle license plate
    /// </summary>
    [HttpPut("{id}/license-plate")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MotorcycleDto?>?> UpdateLicensePlate(Guid id, [FromBody] UpdateMotorcycleLicensePlateCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        command = command with { Id = id };
        var result = await _mediator.Send(command, cancellationToken);
        return result?.ToActionResult();
    }

    /// <summary>
    /// Delete motorcycle
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<bool>> DeleteMotorcycle(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = new DeleteMotorcycleCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (result.IsSuccess && !result.Value)
        {
            return NotFound();
        }
        
        return result.ToActionResult();
    }
}
