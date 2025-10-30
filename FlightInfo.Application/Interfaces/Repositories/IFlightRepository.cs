using FlightInfo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Flight entity operations
    /// </summary>
    public interface IFlightRepository
    {
        /// <summary>
        /// Gets a flight by ID
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>Flight entity or null</returns>
        Task<Flight?> GetByIdAsync(int id);

        /// <summary>
        /// Gets all flights
        /// </summary>
        /// <returns>List of flights</returns>
        Task<IEnumerable<Flight>> GetAllAsync();

        /// <summary>
        /// Gets flights by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered flights</returns>
        Task<IEnumerable<Flight>> GetByCriteriaAsync(Expression<Func<Flight, bool>> predicate);

        /// <summary>
        /// Searches flights by origin, destination and date
        /// </summary>
        /// <param name="origin">Origin airport</param>
        /// <param name="destination">Destination airport</param>
        /// <param name="departureDate">Departure date</param>
        /// <returns>Matching flights</returns>
        Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate);

        /// <summary>
        /// Adds a new flight
        /// </summary>
        /// <param name="flight">Flight entity</param>
        /// <returns>Added flight</returns>
        Task<Flight> AddAsync(Flight flight);

        /// <summary>
        /// Updates an existing flight
        /// </summary>
        /// <param name="flight">Flight entity</param>
        /// <returns>Updated flight</returns>
        Task<Flight> UpdateAsync(Flight flight);

        /// <summary>
        /// Deletes a flight
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Checks if flight exists
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <returns>True if exists</returns>
        Task<bool> ExistsAsync(int id);
    }
}

