using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

public enum NameStatus
{
    VIP,
    Standart
}

namespace Domain
{
    public class TicketType
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public NameStatus Name { get; set; }
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public Event Event { get; set; }
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
