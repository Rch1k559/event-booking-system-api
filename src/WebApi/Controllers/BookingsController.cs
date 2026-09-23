using Application.Events.Commands;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BookingsController : ControllerBase
    {
        private readonly ISender _mediator;

        public BookingsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> Bookings([FromBody] ReserveTicketsCommand command)
        {
            // SECURITY: Extract CustomerId directly from the validated JWT token claims (NameIdentifier/sub) 
            // rather than accepting it from the request body payload to prevent user impersonation.
            var getCustomerIdToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Reconstruct the command with the authenticated user ID
            var query = new ReserveTicketsCommand(command.EventId, Guid.Parse(getCustomerIdToken!), command.Items);
            var result = await _mediator.Send(query);

            return Created($"/api/bookings/{result.BookingId}", result);
        }

        [HttpPost("{id}/confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            var confirmIdBooking = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new ConfirmBookingCommand(id, Guid.Parse(confirmIdBooking!));
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
