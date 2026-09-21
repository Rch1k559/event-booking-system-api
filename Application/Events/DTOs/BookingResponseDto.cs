using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.DTOs
{
    public record BookingResponseDto(Guid BookingId, decimal TotalPrice, DateTime ExpiresAt);
}
