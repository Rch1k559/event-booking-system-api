using System;
using System.Collections.Generic;
using System.Text;

public enum UserRole
{
    Admin,
    Organizer,
    Customer
}

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole? Role { get; set; }

        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<Booking> Bookings { get; set; }
    }
}
