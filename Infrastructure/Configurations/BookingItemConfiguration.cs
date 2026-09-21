using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
    {
        public void Configure(EntityTypeBuilder<BookingItem> builder)
        {
            builder.HasKey(bi => bi.Id);

            builder.HasIndex(bi => new { bi.BookingId, bi.TicketTypeId });

            builder.Property(bi => bi.Quantity)
                .IsRequired();

            builder.Property(bi => bi.PricePerItem)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(bi => bi.Booking)
                .WithMany(b => b.BookingItems)
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bi => bi.TicketType)
                .WithMany(tt => tt.BookingItems)
                .HasForeignKey(bi => bi.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
