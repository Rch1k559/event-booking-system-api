using Application.Common.Intefaces;
using Application.Users.Commands;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly ISender _mediator;

        public UserController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var userId = await _mediator.Send(command);

            return Created($"/api/users/{userId}", new {id = userId});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userToken = await _mediator.Send(command);

            return Created($"/api/users/{userToken}", new { token = userToken });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Auth()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetCurrentUserQuery(Guid.Parse(userIdString!));
            var userProfile = await _mediator.Send(query);

            return Ok(userProfile);
        }
    }
}
