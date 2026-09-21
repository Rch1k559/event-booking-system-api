using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Application.Users.Commands
{
    public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName, UserRole? Role) : IRequest<string>;
}
