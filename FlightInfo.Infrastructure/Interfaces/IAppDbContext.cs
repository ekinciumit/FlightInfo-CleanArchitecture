using Microsoft.EntityFrameworkCore;
using FlightInfo.Domain.Entities;

namespace FlightInfo.Infrastructure.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Flight> Flights { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Reservation> Reservations { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<City> Cities { get; set; }
        DbSet<Airport> Airports { get; set; }
        DbSet<Log> Logs { get; set; }
        DbSet<FlightPrice> FlightPrices { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


