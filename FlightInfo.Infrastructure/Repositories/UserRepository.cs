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
    /// User repository implementation using Entity Framework Core
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User entity or null</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>User entity or null</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .ToListAsync();
        }

        /// <summary>
        /// Gets users by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered users</returns>
        public async Task<IEnumerable<User>> GetByCriteriaAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>Added user</returns>
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>Updated user</returns>
        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
            return user;
        }

        /// <summary>
        /// Soft deletes a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            // Soft delete - set IsDeleted flag if exists, otherwise remove
            if (user.GetType().GetProperty("IsDeleted") != null)
            {
                user.GetType().GetProperty("IsDeleted")?.SetValue(user, true);
            }
            else
            {
                _context.Users.Remove(user);
            }
            return true;
        }

        /// <summary>
        /// Checks if user exists
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if exists</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        /// <summary>
        /// Checks if email exists
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>True if exists</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}

