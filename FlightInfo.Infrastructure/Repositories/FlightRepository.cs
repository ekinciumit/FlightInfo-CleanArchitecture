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
    /// Flight repository implementation using Entity Framework Core
    /// </summary>
    public class FlightRepository : IFlightRepository
    {
        private readonly AppDbContext _context;

        public FlightRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a flight by ID
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>Flight entity or null</returns>
        public async Task<Flight?> GetByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.FlightPrices)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Gets all flights
        /// </summary>
        /// <returns>List of flights</returns>
        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await _context.Flights
                .Include(f => f.FlightPrices)
                .ToListAsync();
        }

        /// <summary>
        /// Gets flights by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered flights</returns>
        public async Task<IEnumerable<Flight>> GetByCriteriaAsync(Expression<Func<Flight, bool>> predicate)
        {
            return await _context.Flights
                .Include(f => f.FlightPrices)
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Searches flights by origin, destination and date
        /// </summary>
        /// <param name="origin">Origin airport</param>
        /// <param name="destination">Destination airport</param>
        /// <param name="departureDate">Departure date</param>
        /// <returns>Matching flights</returns>
        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate)
        {
            var startDate = departureDate.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Flights
                .Include(f => f.FlightPrices)
                .Where(f => f.Origin == origin &&
                           f.Destination == destination &&
                           f.DepartureTime >= startDate &&
                           f.DepartureTime < endDate)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new flight
        /// </summary>
        /// <param name="flight">Flight entity</param>
        /// <returns>Added flight</returns>
        public async Task<Flight> AddAsync(Flight flight)
        {
            _context.Flights.Add(flight);
            return flight;
        }

        /// <summary>
        /// Updates an existing flight
        /// </summary>
        /// <param name="flight">Flight entity</param>
        /// <returns>Updated flight</returns>
        public async Task<Flight> UpdateAsync(Flight flight)
        {
            _context.Flights.Update(flight);
            return flight;
        }

        /// <summary>
        /// Deletes a flight
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
                return false;

            _context.Flights.Remove(flight);
            return true;
        }

        /// <summary>
        /// Checks if flight exists
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>True if exists</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Flights.AnyAsync(f => f.Id == id);
        }
    }
}

