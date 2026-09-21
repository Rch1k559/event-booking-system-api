using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.DTOs
{
    public record ReverseTicketItemDto(Guid TicketTypeId, int Quantity);
}
