using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_rental.api.Middleware;
using vehicle_rental.api.Models;
using vehicle_rental.application.Features.DeliveryPersons.Commands;
using vehicle_rental.application.Features.DeliveryPersons.Queries;
using vehicle_rental.domain.Domain.deliveryPersons.dtos;

namespace vehicle_rental.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryPersonsController(IMediator mediator, ILogger<DeliveryPersonsController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DeliveryPersonsController> _logger = logger;

    /// <summary>
    /// Create a new delivery person
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<DeliveryPersonDto>?> CreateDeliveryPerson([FromBody] CreateDeliveryPersonCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get delivery person by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryPersonDto?>> GetDeliveryPerson(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetDeliveryPersonByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }
        
        return result.ToActionResult();
    }

    /// <summary>
    /// Get all delivery persons
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<DeliveryPersonDto>>?> GetDeliveryPersons(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetDeliveryPersonsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Upload license image for delivery person
    /// </summary>
    [HttpPost("{id}/license-image")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<DeliveryPersonDto>?> UploadLicenseImage(Guid id, [FromForm] LicenseImageUploadModel uploadModel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = new UpdateDeliveryPersonLicenseImageCommand
        {
            Id = id,
            LicenseImage = uploadModel.LicenseImage
        };

        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
