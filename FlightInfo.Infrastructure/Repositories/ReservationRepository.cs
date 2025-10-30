using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Domain.Enums;
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
    /// Reservation repository implementation using Entity Framework Core
    /// </summary>
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a reservation by ID
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>Reservation entity or null</returns>
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Flight)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Gets all reservations
        /// </summary>
        /// <returns>List of reservations</returns>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Flight)
                .Include(r => r.User)
                .ToListAsync();
        }

        /// <summary>
        /// Gets reservations by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User's reservations</returns>
        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.Flight)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets reservations by flight ID
        /// </summary>
        /// <param name="flightId">Flight ID</param>
        /// <returns>Flight's reservations</returns>
        public async Task<IEnumerable<Reservation>> GetByFlightIdAsync(int flightId)
        {
            return await _context.Reservations
                .Include(r => r.Flight)
                .Include(r => r.User)
                .Where(r => r.FlightId == flightId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets reservations by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered reservations</returns>
        public async Task<IEnumerable<Reservation>> GetByCriteriaAsync(Expression<Func<Reservation, bool>> predicate)
        {
            return await _context.Reservations
                .Include(r => r.Flight)
                .Include(r => r.User)
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new reservation
        /// </summary>
        /// <param name="reservation">Reservation entity</param>
        /// <returns>Added reservation</returns>
        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            return reservation;
        }

        /// <summary>
        /// Updates an existing reservation
        /// </summary>
        /// <param name="reservation">Reservation entity</param>
        /// <returns>Updated reservation</returns>
        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return reservation;
        }

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return false;

            _context.Reservations.Remove(reservation);
            return true;
        }

        /// <summary>
        /// Checks if reservation exists
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>True if exists</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservations.AnyAsync(r => r.Id == id);
        }

        /// <summary>
        /// Checks if user has active reservation for flight
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="flightId">Flight ID</param>
        /// <returns>True if has active reservation</returns>
        public async Task<bool> HasActiveReservationAsync(int userId, int flightId)
        {
            return await _context.Reservations
                .AnyAsync(r => r.UserId == userId &&
                             r.FlightId == flightId &&
                             (r.Status == "Pending" ||
                              r.Status == "Confirmed"));
        }
    }
}

