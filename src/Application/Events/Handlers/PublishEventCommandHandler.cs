using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.Notification;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Handlers
{
    public class PublishEventCommandHandler : IRequestHandler<PublishEventCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public PublishEventCommandHandler(IApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(PublishEventCommand request, CancellationToken cancellationToken)
        {
            var eventId = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.EventId);

            if (eventId == null)
            {
                throw new Exception("Event not found!");
            }
            else if (eventId.OrganizerId != request.UserId)
            {
                throw new Exception("Access denied");
            }
            else if (eventId.Status != StatusEvent.Draft)
            {
                throw new Exception("Can only publish drafts");
            }

            eventId.Status = StatusEvent.Published;

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new EventPublishedNotification(request.EventId, eventId.Title), cancellationToken);

            return eventId.Id;
        }
    }
}
