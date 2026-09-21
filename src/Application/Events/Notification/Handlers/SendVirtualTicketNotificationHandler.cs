using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Notification.Handlers
{
    public class SendVirtualTicketNotificationHandler : INotificationHandler<BookingConfirmedNotification>
    {
        private readonly ILogger<SendVirtualTicketNotificationHandler> _logger;

        public SendVirtualTicketNotificationHandler(ILogger<SendVirtualTicketNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(BookingConfirmedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"The virtual ticket for booking {notification.BookingId} has been successfully generated and sent to your email address.");
        }
    }
}
