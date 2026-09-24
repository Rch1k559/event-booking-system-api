using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            if (request.Items == null || !request.Items.Any())
            {
                throw new Exception("Не выбрано ни одного билета для бронирования.");
            }

            if (request.Items.Any(i => i.Quantity <= 0))
            {
                throw new Exception("Количество билетов должно быть больше нуля.");
            }

            const int maxRetries = 3;
            var ticketIds = request.Items.Select(i => i.TicketTypeId).Distinct().ToList();

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    _context.ChangeTracker.Clear();

                    var foundEvent = await _context.Events
                        .FirstOrDefaultAsync(e => e.Id == request.EventId && e.Status == StatusEvent.Published, cancellationToken);

                    if (foundEvent == null)
                    {
                        throw new Exception($"Событие с ID {request.EventId} не найдено или еще не опубликовано.");
                    }

                    var foundTickets = await _context.TicketTypes
                        .Where(tt => ticketIds.Contains(tt.Id))
                        .ToListAsync(cancellationToken);

                    if (foundTickets.Count != ticketIds.Count)
                    {
                        throw new Exception("Один или несколько типов билетов не найдены.");
                    }

                    decimal totalPrice = 0;
                    var bookingItems = new List<BookingItem>();

                    foreach (var item in request.Items)
                    {
                        var ticket = foundTickets.First(t => t.Id == item.TicketTypeId);

                        if (ticket.EventId != request.EventId)
                        {
                            throw new Exception($"Билет {ticket.Name} не принадлежит данному мероприятию.");
                        }

                        if (ticket.AvailableQuantity < item.Quantity)
                        {
                            throw new Exception($"Недостаточно билетов категории '{ticket.Name}'. Доступно: {ticket.AvailableQuantity}.");
                        }

                        ticket.AvailableQuantity -= item.Quantity;

                        totalPrice += ticket.Price * item.Quantity;

                        bookingItems.Add(new BookingItem
                        {
                            TicketTypeId = item.TicketTypeId,
                            Quantity = item.Quantity,
                            PricePerItem = ticket.Price
                        });
                    }

                    var booking = new Booking
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = request.CustomerId,
                        EventId = request.EventId,
                        TotalPrice = totalPrice,
                        Status = StatusBooking.Pending,
                        CreatedAt = DateTime.UtcNow,
                        BookingItems = bookingItems
                    };

                    _context.Bookings.Add(booking);

                    await _context.SaveChangesAsync(cancellationToken);

                    return new BookingResponseDto(
                        BookingId: booking.Id,
                        TotalPrice: booking.TotalPrice,
                        ExpiresAt: booking.CreatedAt.AddMinutes(15)
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (attempt == maxRetries - 1)
                    {
                        throw new Exception("Слишком много одновременных заказов. Пожалуйста, попробуйте снова через пару секунд.");
                    }

                    await Task.Delay(50 * (attempt + 1), cancellationToken);
                }
            }

            throw new Exception("Не удалось завершить бронирование. Попробуйте снова.");
        }
    }
}