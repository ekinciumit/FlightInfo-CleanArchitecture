using FlightInfo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Log repository interface
    /// </summary>
    public interface ILogRepository : IRepository<Log>
    {
        /// <summary>
        /// Gets logs by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of logs</returns>
        Task<IEnumerable<Log>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Gets logs by flight ID
        /// </summary>
        /// <param name="flightId">Flight ID</param>
        /// <returns>List of logs</returns>
        Task<IEnumerable<Log>> GetByFlightIdAsync(int flightId);

        /// <summary>
        /// Gets logs by level
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>List of logs</returns>
        Task<IEnumerable<Log>> GetByLevelAsync(string level);

        /// <summary>
        /// Gets logs by date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of logs</returns>
        Task<IEnumerable<Log>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}

