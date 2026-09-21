using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.Handlers;
using Application.UnitTests.Common;
using Domain;
using FluentAssertions;

namespace Application.UnitTests.Events
{
    public class ConfirmBookingCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ConfirmBookingCommandHandler _handler;

        public ConfirmBookingCommandHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _handler = new ConfirmBookingCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ValidPendingBooking_ShouldSetStatusConfirmed()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ConfirmBookingCommand(booking.Id, customerId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(booking.Id);
            var updatedBooking = await _context.Bookings.FindAsync(booking.Id);
            updatedBooking!.Status.Should().Be(StatusBooking.Confirmed);
        }

        [Fact]
        public async Task Handle_AnotherCustomer_ShouldThrowException()
        {
            // Arrange
            var realCustomer = Guid.NewGuid();
            var impostorCustomer = Guid.NewGuid();
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = realCustomer,
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ConfirmBookingCommand(booking.Id, impostorCustomer);

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("You cannot confirm someone else’s booking");
        }

        [Fact]
        public async Task Handle_BookingOlderThan15Minutes_ShouldThrowExpiredException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = StatusBooking.Pending,
                CreatedAt = DateTime.UtcNow.AddMinutes(-16)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ConfirmBookingCommand(booking.Id, customerId);

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("The booking has expired");
        }
    }
}
