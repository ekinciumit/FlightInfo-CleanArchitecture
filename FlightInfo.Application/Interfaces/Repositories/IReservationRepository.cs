using FlightInfo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Reservation entity operations
    /// </summary>
    public interface IReservationRepository
    {
        /// <summary>
        /// Gets a reservation by ID
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>Reservation entity or null</returns>
        Task<Reservation?> GetByIdAsync(int id);

        /// <summary>
        /// Gets all reservations
        /// </summary>
        /// <returns>List of reservations</returns>
        Task<IEnumerable<Reservation>> GetAllAsync();

        /// <summary>
        /// Gets reservations by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User's reservations</returns>
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Gets reservations by flight ID
        /// </summary>
        /// <param name="flightId">Flight ID</param>
        /// <returns>Flight's reservations</returns>
        Task<IEnumerable<Reservation>> GetByFlightIdAsync(int flightId);

        /// <summary>
        /// Gets reservations by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered reservations</returns>
        Task<IEnumerable<Reservation>> GetByCriteriaAsync(Expression<Func<Reservation, bool>> predicate);

        /// <summary>
        /// Adds a new reservation
        /// </summary>
        /// <param name="reservation">Reservation entity</param>
        /// <returns>Added reservation</returns>
        Task<Reservation> AddAsync(Reservation reservation);

        /// <summary>
        /// Updates an existing reservation
        /// </summary>
        /// <param name="reservation">Reservation entity</param>
        /// <returns>Updated reservation</returns>
        Task<Reservation> UpdateAsync(Reservation reservation);

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Checks if reservation exists
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>True if exists</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Checks if user has active reservation for flight
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="flightId">Flight ID</param>
        /// <returns>True if has active reservation</returns>
        Task<bool> HasActiveReservationAsync(int userId, int flightId);
    }
}

