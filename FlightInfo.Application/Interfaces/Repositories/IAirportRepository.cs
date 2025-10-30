using FlightInfo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Airport repository interface
    /// </summary>
    public interface IAirportRepository : IRepository<Airport>
    {
        /// <summary>
        /// Gets airports by city ID
        /// </summary>
        /// <param name="cityId">City ID</param>
        /// <returns>List of airports</returns>
        Task<IEnumerable<Airport>> GetByCityIdAsync(int cityId);

        /// <summary>
        /// Gets airport by code
        /// </summary>
        /// <param name="code">Airport code</param>
        /// <returns>Airport entity or null</returns>
        Task<Airport?> GetByCodeAsync(string code);

        /// <summary>
        /// Gets airports by country code
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <returns>List of airports</returns>
        Task<IEnumerable<Airport>> GetByCountryCodeAsync(string countryCode);
    }
}

