using Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Queries
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<UserProfileDto>;
}
