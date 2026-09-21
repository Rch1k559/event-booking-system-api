using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Notification
{
    public record EventPublishedNotification(Guid EventId, string Title) : INotification;
}
