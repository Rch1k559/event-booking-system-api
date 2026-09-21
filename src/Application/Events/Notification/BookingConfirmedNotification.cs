using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Notification
{
    public record BookingConfirmedNotification(Guid BookingId) : INotification;
}
