using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class BookingItem
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }

        public Booking Booking { get; set; }
        public TicketType TicketType { get; set; }
    }
}
