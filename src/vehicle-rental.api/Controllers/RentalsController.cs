using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_rental.api.Middleware;
using vehicle_rental.application.Features.Rentals.Commands;
using vehicle_rental.application.Features.Rentals.Queries;
using vehicle_rental.domain.Domain.rentals.dtos;

namespace vehicle_rental.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RentalsController(IMediator mediator, ILogger<RentalsController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<RentalsController> _logger = logger;

    /// <summary>
    /// Create a new rental
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RentalDto>?> CreateRental([FromBody] CreateRentalCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _mediator.Send(command, cancellationToken);
        return result?.ToActionResult();
    }

    /// <summary>
    /// Get rental by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDto?>?> GetRental(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetRentalByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.IsSuccess && result.Value == null)
        {
            return NotFound();
        }
        
        return result?.ToActionResult();
    }

    /// <summary>
    /// Get rentals by delivery person
    /// </summary>
    [HttpGet("delivery-person/{deliveryPersonId}")]
    public async Task<ActionResult<IEnumerable<RentalDto>>?> GetRentalsByDeliveryPerson(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new GetRentalsByDeliveryPersonQuery { DeliveryPersonId = deliveryPersonId };
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Calculate rental return amount
    /// </summary>
    [HttpPost("{id}/calculate-return")]
    public async Task<ActionResult<RentalCalculationDto>?> CalculateRentalReturn(Guid id, [FromBody] DateTime returnDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = new CalculateRentalReturnQuery { RentalId = id, ReturnDate = returnDate };
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Complete rental
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<RentalDto>?> CompleteRental(Guid id, [FromBody] DateTime returnDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = new CompleteRentalCommand { Id = id, ReturnDate = returnDate };
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
