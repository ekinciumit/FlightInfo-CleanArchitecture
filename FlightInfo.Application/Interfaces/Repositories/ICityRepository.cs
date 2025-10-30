using FlightInfo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// City repository interface
    /// </summary>
    public interface ICityRepository : IRepository<City>
    {
        /// <summary>
        /// Gets cities by country ID
        /// </summary>
        /// <param name="countryId">Country ID</param>
        /// <returns>List of cities</returns>
        Task<IEnumerable<City>> GetByCountryIdAsync(int countryId);

        /// <summary>
        /// Gets city by code
        /// </summary>
        /// <param name="code">City code</param>
        /// <returns>City entity or null</returns>
        Task<City?> GetByCodeAsync(string code);

        /// <summary>
        /// Gets all cities with airports
        /// </summary>
        /// <returns>List of cities with airports</returns>
        Task<IEnumerable<City>> GetAllWithAirportsAsync();
    }
}

