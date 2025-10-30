using FlightInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightInfo.Infrastructure.Data.Configurations
{
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.FlightNumber).IsRequired().HasMaxLength(20);
            builder.Property(f => f.Origin).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Destination).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Status).IsRequired().HasMaxLength(20);
            builder.Property(f => f.DepartureTime).IsRequired();
            builder.Property(f => f.ArrivalTime).IsRequired();
            builder.Property(f => f.AircraftType).HasMaxLength(50);
            builder.Property(f => f.Capacity).IsRequired();
            builder.Property(f => f.AvailableSeats).IsRequired();
            builder.Property(f => f.CreatedAt).IsRequired();
            builder.Property(f => f.UpdatedAt);
            builder.Property(f => f.IsDeleted).IsRequired().HasDefaultValue(false);

            // Relationships
            builder.HasMany(f => f.Reservations)
                .WithOne(r => r.Flight)
                .HasForeignKey(r => r.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.FlightPrices)
                .WithOne(fp => fp.Flight)
                .HasForeignKey(fp => fp.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            builder.HasIndex(f => new { f.FlightNumber, f.DepartureTime }).IsUnique();
            builder.HasIndex(f => f.FlightNumber);
        }
    }
}
