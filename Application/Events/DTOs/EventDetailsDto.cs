using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.DTOs
{
    public record EventDetailsDto(Guid Id, string OrganizerName, string Title, string Description, DateTime Date, string Location, List<TicketTypeDetailsDto> TicketTypes);
}
