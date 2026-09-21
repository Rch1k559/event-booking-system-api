using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.Handlers;
using Application.UnitTests.Common;
using Domain;
using FluentAssertions;

namespace Application.UnitTests.Events
{
    public class CancelExpiredBookingsCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly CancelExpiredBookingsCommandHandler _handler;

        public CancelExpiredBookingsCommandHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _handler = new CancelExpiredBookingsCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ExpiredPendingBookings_ShouldCancelAndRestoreTicketQuantity()
        {
            var ticketTypeId = Guid.NewGuid();
            var ticketType = new TicketType
            {
                Id = ticketTypeId,
                Name = NameStatus.Standart,
                Price = 50m,
                TotalQuantity = 100,
                AvailableQuantity = 90 
            };
            _context.TicketTypes.Add(ticketType);

            var expiredBooking = new Booking
            {
                Id = Guid.NewGuid(),
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow.AddMinutes(-20),
                BookingItems = new List<BookingItem>
            {
                new() { TicketTypeId = ticketTypeId, Quantity = 10, PricePerItem = 50m }
            }
            };

            var freshBooking = new Booking
            {
                Id = Guid.NewGuid(),
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                BookingItems = new List<BookingItem>()
            };

            _context.Bookings.AddRange(expiredBooking, freshBooking);
            await _context.SaveChangesAsync(CancellationToken.None);
            await _handler.Handle(new CancelExpiredBookingsCommand(), CancellationToken.None);

            var updatedExpiredBooking = await _context.Bookings.FindAsync(expiredBooking.Id);
            updatedExpiredBooking!.Status.Should().Be(StatusBooking.Cancelled);

            var updatedFreshBooking = await _context.Bookings.FindAsync(freshBooking.Id);
            updatedFreshBooking!.Status.Should().Be(StatusBooking.Pending);

            var updatedTicket = await _context.TicketTypes.FindAsync(ticketTypeId);
            updatedTicket!.AvailableQuantity.Should().Be(100);
        }
    }
}
