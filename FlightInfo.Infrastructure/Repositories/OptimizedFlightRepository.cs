using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightInfo.Infrastructure.Repositories
{
    /// <summary>
    /// Optimized Flight repository with performance improvements
    /// </summary>
    public class OptimizedFlightRepository : IFlightRepository
    {
        private readonly AppDbContext _context;

        public OptimizedFlightRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a flight by ID with optimized query
        /// </summary>
        public async Task<Flight?> GetByIdAsync(int id)
        {
            return await _context.Flights
                .AsNoTracking() // Read-only için tracking kapat
                .Include(f => f.FlightPrices)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Gets all flights with pagination
        /// </summary>
        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await _context.Flights
                .AsNoTracking()
                .Include(f => f.FlightPrices)
                .Where(f => !f.IsDeleted)
                .OrderBy(f => f.DepartureTime)
                .ToListAsync();
        }

        /// <summary>
        /// Gets flights with pagination for better performance
        /// </summary>
        public async Task<(IEnumerable<Flight> Flights, int TotalCount)> GetFlightsPagedAsync(
            int pageNumber,
            int pageSize,
            string? origin = null,
            string? destination = null)
        {
            var query = _context.Flights
                .AsNoTracking()
                .Include(f => f.FlightPrices)
                .Where(f => !f.IsDeleted);

            if (!string.IsNullOrEmpty(origin))
            {
                query = query.Where(f => f.Origin == origin);
            }

            if (!string.IsNullOrEmpty(destination))
            {
                query = query.Where(f => f.Destination == destination);
            }

            var totalCount = await query.CountAsync();

            var flights = await query
                .OrderBy(f => f.DepartureTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (flights, totalCount);
        }

        /// <summary>
        /// Gets flights by criteria with optimized query
        /// </summary>
        public async Task<IEnumerable<Flight>> GetByCriteriaAsync(Expression<Func<Flight, bool>> criteria)
        {
            return await _context.Flights
                .AsNoTracking()
                .Include(f => f.FlightPrices)
                .Where(criteria)
                .Where(f => !f.IsDeleted)
                .OrderBy(f => f.DepartureTime)
                .ToListAsync();
        }

        /// <summary>
        /// Searches flights by origin, destination and date
        /// </summary>
        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate)
        {
            return await _context.Flights
                .AsNoTracking()
                .Include(f => f.FlightPrices)
                .Where(f => f.Origin == origin && f.Destination == destination)
                .Where(f => f.DepartureTime.Date == departureDate.Date)
                .Where(f => !f.IsDeleted)
                .OrderBy(f => f.DepartureTime)
                .ToListAsync();
        }

        /// <summary>
        /// Gets flights by date range with optimized query
        /// </summary>
        public async Task<IEnumerable<Flight>> GetFlightsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            string? origin = null,
            string? destination = null)
        {
            var query = _context.Flights
                .AsNoTracking()
                .Include(f => f.FlightPrices)
                .Where(f => f.DepartureTime >= startDate && f.DepartureTime <= endDate)
                .Where(f => !f.IsDeleted);

            if (!string.IsNullOrEmpty(origin))
            {
                query = query.Where(f => f.Origin == origin);
            }

            if (!string.IsNullOrEmpty(destination))
            {
                query = query.Where(f => f.Destination == destination);
            }

            return await query
                .OrderBy(f => f.DepartureTime)
                .ToListAsync();
        }

        /// <summary>
        /// Gets flight statistics for dashboard
        /// </summary>
        public async Task<object> GetFlightStatisticsAsync()
        {
            var stats = await _context.Flights
                .AsNoTracking()
                .Where(f => !f.IsDeleted)
                .GroupBy(f => f.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalFlights = await _context.Flights
                .AsNoTracking()
                .Where(f => !f.IsDeleted)
                .CountAsync();

            return new
            {
                TotalFlights = totalFlights,
                StatusBreakdown = stats,
                LastUpdated = DateTime.UtcNow
            };
        }

        // Diğer metodlar için base repository'den implementasyon
        public async Task<Flight> AddAsync(Flight flight)
        {
            await _context.Flights.AddAsync(flight);
            return flight;
        }

        public async Task<Flight> UpdateAsync(Flight flight)
        {
            _context.Flights.Update(flight);
            return flight;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                flight.IsDeleted = true;
                _context.Flights.Update(flight);
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Flights
                .AsNoTracking()
                .AnyAsync(f => f.Id == id && !f.IsDeleted);
        }
    }
}

