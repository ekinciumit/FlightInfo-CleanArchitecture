using FlightInfo.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Services
{
    /// <summary>
    /// City service interface
    /// </summary>
    public interface ICityService
    {
        /// <summary>
        /// Gets all cities
        /// </summary>
        /// <returns>List of cities</returns>
        Task<IEnumerable<CityDto>> GetCitiesAsync();

        /// <summary>
        /// Gets cities by country ID
        /// </summary>
        /// <param name="countryId">Country ID</param>
        /// <returns>List of cities</returns>
        Task<IEnumerable<CityDto>> GetCitiesByCountryIdAsync(int countryId);

        /// <summary>
        /// Gets city by ID
        /// </summary>
        /// <param name="id">City ID</param>
        /// <returns>City DTO or null</returns>
        Task<CityDto?> GetCityAsync(int id);

        /// <summary>
        /// Gets city by code
        /// </summary>
        /// <param name="code">City code</param>
        /// <returns>City DTO or null</returns>
        Task<CityDto?> GetCityByCodeAsync(string code);
    }
}

