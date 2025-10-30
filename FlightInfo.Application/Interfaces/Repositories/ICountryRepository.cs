using FlightInfo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Repositories
{
    /// <summary>
    /// Country repository interface
    /// </summary>
    public interface ICountryRepository : IRepository<Country>
    {
        /// <summary>
        /// Gets country by code
        /// </summary>
        /// <param name="code">Country code</param>
        /// <returns>Country entity or null</returns>
        Task<Country?> GetByCodeAsync(string code);

        /// <summary>
        /// Gets all countries with cities
        /// </summary>
        /// <returns>List of countries with cities</returns>
        Task<IEnumerable<Country>> GetAllWithCitiesAsync();
    }
}

