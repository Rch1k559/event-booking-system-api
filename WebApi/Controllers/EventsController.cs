using Application.Events.Commands;
using Application.Events.Queries;
using Application.Users.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class EventsController : ControllerBase
    {
        private readonly ISender _mediator;

        public EventsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        [Authorize(Roles = "Organizer, Admin")]
        public async Task<IActionResult> EventsCreate([FromBody] CreateEventCommand command)
        {
            var organizerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new CreateEventCommand(command.Title, command.Description, command.Date, command.Location, Guid.Parse(organizerIdString!), command.TicketTypes);
            var eventCreate = await _mediator.Send(query);

            return Created($"/api/events/{eventCreate}", new {id = eventCreate});
        }

        [HttpPost("{id}/publish")]
        [Authorize]
        public async Task<IActionResult> PublishEvent(Guid id)
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new PublishEventCommand(id, Guid.Parse(organizerId!));
            var publishEvent = await _mediator.Send(query);

            return Ok(publishEvent);
        }

        [HttpGet()]
        public async Task<IActionResult> GetEvent([FromQuery] GetPublishedEventsQuery command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventDetails(Guid id)
        {
            var query = new GetEventDetailsQuery(id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
