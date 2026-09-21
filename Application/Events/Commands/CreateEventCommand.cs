using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Commands
{
    public record CreateEventCommand(string Title, string Description, DateTime Date, string Location, Guid OrganizerId, List<CreateTicketTypeDto> TicketTypes) : IRequest<Guid>;
}
