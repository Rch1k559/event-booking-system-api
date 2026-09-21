using Application.Common.Intefaces;
using Application.Events.Commands;
using Application.Events.DTOs;
using Application.Events.Handlers;
using Application.UnitTests.Common;
using Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UnitTests.Events
{
    public class ReserveTicketsCommandHandlerTests
    {
        private readonly IApplicationDbContext _context;
        private readonly ReserveTicketsCommandHandler _handler;

        public ReserveTicketsCommandHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _handler = new ReserveTicketsCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReserveTicketsAndReduceAvailability()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();

            var ev = new Event
            {
                Id = eventId,
                Title = "Rock Concert",
                Description = "Live music",
                Date = DateTime.UtcNow.AddDays(7),
                Location = "Main Arena",
                Status = StatusEvent.Published,
                OrganizerId = Guid.NewGuid()
            };

            var ticketType = new TicketType
            {
                Id = ticketTypeId,
                EventId = eventId,
                Name = NameStatus.Standart,
                Price = 100m,
                TotalQuantity = 50,
                AvailableQuantity = 50
            };

            _context.Events.Add(ev);
            _context.TicketTypes.Add(ticketType);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ReserveTicketsCommand(
                eventId,
                customerId,
                new List<ReverseTicketItemDto> { new(ticketTypeId, 2) }
            );

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.TotalPrice.Should().Be(200m);

            var updatedTicket = await _context.TicketTypes.FindAsync(ticketTypeId);
            updatedTicket!.AvailableQuantity.Should().Be(48);

            var booking = await _context.Bookings.FindAsync(result.BookingId);
            booking.Should().NotBeNull();
            booking!.Status.Should().Be(StatusBooking.Pending);
            booking.BookingItems.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_InsufficientTickets_ShouldThrowException()
        {
            var eventId = Guid.NewGuid();
            var ticketTypeId = Guid.NewGuid();

            _context.Events.Add(new Event
            {
                Id = eventId,
                Title = "Conference",
                Description = "Tech talk",
                Date = DateTime.UtcNow.AddDays(1),
                Location = "Hall 1",
                Status = StatusEvent.Published
            });

            _context.TicketTypes.Add(new TicketType
            {
                Id = ticketTypeId,
                EventId = eventId,
                Name = NameStatus.VIP,
                Price = 500m,
                TotalQuantity = 1,
                AvailableQuantity = 1
            });
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ReserveTicketsCommand(
                eventId,
                Guid.NewGuid(),
                new List<ReverseTicketItemDto> { new(ticketTypeId, 3) }
            );

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Insufficient tickets available");
        }

        [Fact]
        public async Task Handle_EventNotPublished_ShouldThrowException()
        {
            var eventId = Guid.NewGuid();
            _context.Events.Add(new Event
            {
                Id = eventId,
                Title = "Draft event",
                Description = "Secret",
                Date = DateTime.UtcNow.AddDays(5),
                Location = "Somewhere",
                Status = StatusEvent.Draft
            });
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new ReserveTicketsCommand(eventId, Guid.NewGuid(), new List<ReverseTicketItemDto>());

            // Act & Assert
            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
