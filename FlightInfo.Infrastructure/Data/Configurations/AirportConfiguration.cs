using FlightInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightInfo.Infrastructure.Data.Configurations
{
    public class AirportConfiguration : IEntityTypeConfiguration<Airport>
    {
        public void Configure(EntityTypeBuilder<Airport> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Code).IsRequired().HasMaxLength(10);
            builder.Property(a => a.Name).IsRequired().HasMaxLength(255);
            builder.Property(a => a.FullName).IsRequired().HasMaxLength(500);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UpdatedAt);
            builder.Property(a => a.IsDeleted).IsRequired().HasDefaultValue(false);

            // Indexes
            builder.HasIndex(a => a.Code).IsUnique();

            // Relationships
            builder.HasOne(a => a.City)
                .WithMany(c => c.Airports)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
