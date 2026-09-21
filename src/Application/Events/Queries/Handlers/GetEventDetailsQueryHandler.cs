using Application.Common.Intefaces;
using Application.Events.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RA.Utilities.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Queries.Handlers
{
    public class GetEventDetailsQueryHandler : IRequestHandler<GetEventDetailsQuery, EventDetailsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetEventDetailsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EventDetailsDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Events.AsNoTracking().Where(e => e.Id == request.EventId && e.Status != StatusEvent.Draft);

            var result = await query.Include(e => e.TicketTypes).Select(e => new EventDetailsDto
            (
                e.Id,
                e.Organizer.Email,
                e.Title,
                e.Description,
                e.Date,
                e.Location,
                e.TicketTypes.Select(tt => new TicketTypeDetailsDto(tt.Id, tt.Name.ToString(), tt.Price, tt.TotalQuantity, tt.AvailableQuantity)).ToList()
            )).FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(nameof(Events), request.EventId);
            }

            return result;
        }
    }
}
