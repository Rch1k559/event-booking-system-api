using Application.Common.Intefaces;
using Application.Users.DTOs;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Handlers
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
    {
        private readonly IApplicationDbContext _context;

        public GetCurrentUserQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new Exception("User not found!");
            }

            return new UserProfileDto(user.Id, user.Email, user.Role.ToString());
        }
    }
}
