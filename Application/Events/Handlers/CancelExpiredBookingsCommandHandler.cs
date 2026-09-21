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
            // Расчет временного порога: брони в статусе Pending старше 15 минут считаются просроченными
            DateTime threshold = DateTime.UtcNow.AddMinutes(-15);

            var expiredBooking = await _context.Bookings.Include(b => b.BookingItems)// Обязательно подгружаем позиции бронирования
                .Where(b => b.Status == StatusBooking.Pending && b.CreatedAt < threshold).ToListAsync(cancellationToken);
            // Возвращаем зарезервированное количество билетов обратно в доступный остаток
            foreach (var booking in expiredBooking)
            {
                booking.Status = StatusBooking.Cancelled;
                foreach(var item in booking.BookingItems)
                {
                    var foundTicket = await _context.TicketTypes.FirstOrDefaultAsync(tt => tt.Id == item.TicketTypeId);

                    foundTicket.AvailableQuantity += item.Quantity;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
