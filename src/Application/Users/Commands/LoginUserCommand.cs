using Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands
{
    public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
