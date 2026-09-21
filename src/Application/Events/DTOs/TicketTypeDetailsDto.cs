using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.DTOs
{
    public record TicketTypeDetailsDto(Guid Id, string Name, decimal Price, int TotalQuantity, int AvailableQuantity);
}
