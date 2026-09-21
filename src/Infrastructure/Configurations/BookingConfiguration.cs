using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasIndex(b => b.CustomerId);

            builder.HasIndex(b => new { b.Status, b.CreatedAt });

            builder.HasIndex(b => b.EventId);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(b => b.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
