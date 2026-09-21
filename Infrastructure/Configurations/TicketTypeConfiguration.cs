using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
    {
        public void Configure(EntityTypeBuilder<TicketType> builder)
        {
            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.EventId)
                .IsRequired();

            builder.HasIndex(tt => tt.AvailableQuantity);

            builder.Property(tt => tt.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasIndex(tt => tt.Name);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();
        }
    }
}
