using System;
using System.Collections.Generic;
using System.Text;

public enum StatusEvent
{
    Draft,
    Published,
    Cancelled
}

namespace Domain
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public StatusEvent Status { get; set; }
        public Guid OrganizerId { get; set; }

        public User Organizer { get; set; } = null!;
        public ICollection<TicketType> TicketTypes { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
