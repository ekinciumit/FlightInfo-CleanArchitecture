using FlightInfo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for User entity operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User entity or null</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>User entity or null</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Gets users by criteria
        /// </summary>
        /// <param name="predicate">Filter criteria</param>
        /// <returns>Filtered users</returns>
        Task<IEnumerable<User>> GetByCriteriaAsync(Expression<Func<User, bool>> predicate);

        /// <summary>
        /// Adds a new user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>Added user</returns>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>Updated user</returns>
        Task<User> UpdateAsync(User user);

        /// <summary>
        /// Soft deletes a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Checks if user exists
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if exists</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Checks if email exists
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>True if exists</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}

