using FlightInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightInfo.Infrastructure.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.PassengerName).IsRequired().HasMaxLength(255);
            builder.Property(r => r.PassengerEmail).IsRequired().HasMaxLength(255);
            builder.Property(r => r.PassengerPhone).IsRequired().HasMaxLength(20);
            builder.Property(r => r.SeatNumber).IsRequired().HasMaxLength(10);
            builder.Property(r => r.Class).IsRequired().HasMaxLength(20);
            builder.Property(r => r.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(r => r.Currency).IsRequired().HasMaxLength(3);
            builder.Property(r => r.Status).IsRequired().HasMaxLength(20);
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt);
            builder.Property(r => r.IsDeleted).IsRequired().HasDefaultValue(false);

            // Relationships
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Flight)
                .WithMany(f => f.Reservations)
                .HasForeignKey(r => r.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            builder.HasIndex(r => new { r.FlightId, r.SeatNumber }).IsUnique();
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.FlightId);
        }
    }
}
