using Application.Common.Intefaces;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Notification.Handlers
{
    public class AuditLogHandler : INotificationHandler<EventPublishedNotification>
    {
        private readonly IApplicationDbContext _context;

        public AuditLogHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(EventPublishedNotification notification, CancellationToken cancellationToken)
        {
            var auditLog = new AuditLog
            {
                Action = "EventPublished",
                EntityId = notification.EventId,
                Details = $"The “{notification.Title}” event has been successfully published",
                TimeStamp = DateTime.UtcNow,
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
