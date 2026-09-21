using Application.Events.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Queries
{
    public record GetEventDetailsQuery(Guid EventId) : IRequest<EventDetailsDto>;
}
