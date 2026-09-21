using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Notification.Handlers
{
    public class SendSubscriberNotificationsHandler : INotificationHandler<EventPublishedNotification>
    {
        public Task Handle(EventPublishedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Sending a newsletter to subscribers about a new event - {notification.Title}");
            return Task.CompletedTask;
        }
    }
}
