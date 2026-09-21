using Application.Common.Intefaces;
using Application.Events.Commands;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Handlers
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateEventCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Location = request.Location,
                Status = StatusEvent.Draft,
                OrganizerId = request.OrganizerId,
                TicketTypes = request.TicketTypes.Select(t => new TicketType
                {
                    Id = Guid.NewGuid(),
                    Name = Enum.Parse<NameStatus>(t.Name),
                    Price = t.Price,
                    TotalQuantity = t.TotalQuantity,
                    AvailableQuantity = t.TotalQuantity
                }).ToList()
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }
    }
}
