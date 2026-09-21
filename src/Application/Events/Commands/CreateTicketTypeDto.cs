using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Commands
{
    public record CreateTicketTypeDto(string Name, decimal Price, int TotalQuantity);
}
