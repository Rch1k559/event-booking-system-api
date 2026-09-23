using Application.Common.Intefaces;
using Application.Events.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Handlers
{
    public class CancelExpiredBookingsCommandHandler : IRequestHandler<CancelExpiredBookingsCommand>
    {
        private readonly IApplicationDbContext _context;

        public CancelExpiredBookingsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CancelExpiredBookingsCommand request, CancellationToken cancellationToken)
        {
            // Threshold calculation: pending bookings older than 15 minutes are considered expired
            DateTime threshold = DateTime.UtcNow.AddMinutes(-15);

            var expiredBooking = await _context.Bookings.Include(b => b.BookingItems)// Eagerly load booking items to access quantities
                .Where(b => b.Status == StatusBooking.Pending && b.CreatedAt < threshold).ToListAsync(cancellationToken);

            foreach (var booking in expiredBooking)
            {
                booking.Status = StatusBooking.Cancelled;// Mark booking status as Cancelled
                foreach (var item in booking.BookingItems)
                {
                    var foundTicket = await _context.TicketTypes.FirstOrDefaultAsync(tt => tt.Id == item.TicketTypeId);

                    foundTicket.AvailableQuantity += item.Quantity;// Re-add tickets to inventory
                }
            }

            await _context.SaveChangesAsync(cancellationToken);// Save changes in a single transaction
        }
    }
}
