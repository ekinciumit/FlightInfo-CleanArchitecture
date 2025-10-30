using FlightInfo.Domain.Entities;
using FlightInfo.Infrastructure.Interfaces;
using FlightInfo.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlightInfo.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<Log> Logs { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<TwoFactorCode> TwoFactorCodes { get; set; } = null!;

        // Location entities
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Airport> Airports { get; set; } = null!;

        // Flight search entities
        public DbSet<FlightPrice> FlightPrices { get; set; } = null!;
        public DbSet<FlightStatusHistory> FlightStatusHistory { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Global query filters for soft delete
            modelBuilder.Entity<Flight>().HasQueryFilter(f => !f.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Reservation>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Country>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<City>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Airport>().HasQueryFilter(a => !a.IsDeleted);
            // Log ve FlightPrice için IsDeleted property'si yok, bu yüzden query filter eklemiyoruz

            // Apply all configurations
            modelBuilder.ApplyConfiguration(new FlightConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReservationConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new AirportConfiguration());

            // Log -> User (nullable)
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany()                   // User.Logs koleksiyonu yok
                .HasForeignKey(l => l.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Log -> Flight (nullable)
            modelBuilder.Entity<Log>()
                .HasOne(l => l.Flight)
                .WithMany()                   // Flight.Logs koleksiyonu yok
                .HasForeignKey(l => l.FlightId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany() // kullanıcıdan rezervasyon koleksiyonu tutmuyoruz
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Flight)
                .WithMany() // Flight'tan rezervasyon koleksiyonu tutmuyoruz
                .HasForeignKey(r => r.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // Aynı kullanıcı + aynı uçuş için birden fazla aktif rezervasyonu engelle (opsiyonel ama faydalı)
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.UserId, r.FlightId, r.Status });

            // Country -> City -> Airport relationships
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .HasMany(c => c.Airports)
                .WithOne(a => a.City)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Airport>()
                .HasIndex(a => a.Code)
                .IsUnique();

            // FlightPrice decimal precision (sadece uyarıyı gidermek için)
            modelBuilder.Entity<FlightPrice>()
                .Property(fp => fp.Price)
                .HasPrecision(18, 2);

            // Reservation TotalPrice decimal precision
            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}

