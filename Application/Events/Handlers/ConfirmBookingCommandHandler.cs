using Application.Common.Intefaces;
using Application.Events.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Handlers
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public ConfirmBookingCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var foundBookingId = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (foundBookingId == null)
            {
                throw new Exception("No booking found");
            }
            if (foundBookingId.CustomerId != request.CustomerId)
            {
                throw new Exception("You cannot confirm someone else’s booking");
            }
            if (foundBookingId.Status != StatusBooking.Pending)
            {
                throw new Exception("");
            }
            if (DateTime.UtcNow > foundBookingId.CreatedAt.AddMinutes(15))
            {
                throw new Exception("The booking has expired");
            }

            foundBookingId.Status = StatusBooking.Confirmed;

            await _context.SaveChangesAsync(cancellationToken);

            return foundBookingId.Id;
        }
    }
}
