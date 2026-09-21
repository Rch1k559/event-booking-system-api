using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Intefaces
{
    public interface IApplicationDbContext
    {
        DbSet<AuditLog> AuditLogs { get; }
        DbSet <User> Users { get; }
        DbSet<Event> Events { get; }
        DbSet<TicketType> TicketTypes { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingItem> BookingItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
