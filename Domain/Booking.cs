using System;
using System.Collections.Generic;
using System.Text;

public enum StatusBooking
{
    Pending,
    Confirmed,
    Cancelled
}

namespace Domain
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid EventId { get; set; }
        public StatusBooking Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public Event Event { get; set; }
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public User Customer { get; set; }
    }
}
