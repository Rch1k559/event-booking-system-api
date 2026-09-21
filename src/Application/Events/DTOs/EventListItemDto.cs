using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.DTOs
{
    public record EventListItemDto(Guid Id, string Title, DateTime Date, string Location, decimal MinPrice);
}
