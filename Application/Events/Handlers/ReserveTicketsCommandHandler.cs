using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Handlers
{
    public class ReserveTicketsCommandHandler : IRequestHandler<ReserveTicketsCommand, BookingResponseDto>
    {
        private readonly IApplicationDbContext _context;

        public ReserveTicketsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponseDto> Handle(ReserveTicketsCommand request, CancellationToken cancellationToken)
        {
            var foundEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == request.EventId && e.Status == StatusEvent.Published);

            if (foundEvent == null)
            {
                throw new Exception("");
            }

            decimal totalPrice = 0;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                EventId = request.EventId,
                TotalPrice = totalPrice,
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow,
                BookingItems = new List<BookingItem>()
            };

            foreach (var item in request.Items)
            {
                var foundTicket = await _context.TicketTypes.FirstOrDefaultAsync(tt => tt.Id == item.TicketTypeId);

                if (foundTicket == null)
                {
                    throw new Exception("No ticket category found");
                }

                if (foundTicket.AvailableQuantity < item.Quantity)
                {
                    throw new Exception("Insufficient tickets available");
                }

                foundTicket.AvailableQuantity -= item.Quantity;

                totalPrice += foundTicket.Price * item.Quantity;

                var line = new BookingItem
                {
                    TicketTypeId = item.TicketTypeId,
                    Quantity = item.Quantity,
                    PricePerItem = foundTicket.Price
                };

                booking.BookingItems.Add(line);
            }

            booking.TotalPrice = totalPrice;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);

            return new BookingResponseDto(
                BookingId: booking.Id,
                TotalPrice: booking.TotalPrice,
                ExpiresAt: booking.CreatedAt.AddMinutes(15)
                );
        }
    }
}
