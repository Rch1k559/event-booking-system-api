using Application.Events.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Commands
{
    public record ReserveTicketsCommand(Guid EventId, Guid CustomerId, List<ReverseTicketItemDto> Items) : IRequest<BookingResponseDto>;
}
