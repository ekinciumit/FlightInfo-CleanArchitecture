using FlightInfo.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Application.Interfaces.Services
{
    /// <summary>
    /// Airport service interface
    /// </summary>
    public interface IAirportService
    {
        /// <summary>
        /// Gets all airports
        /// </summary>
        /// <returns>List of airports</returns>
        Task<IEnumerable<AirportDto>> GetAirportsAsync();

        /// <summary>
        /// Gets airports by city ID
        /// </summary>
        /// <param name="cityId">City ID</param>
        /// <returns>List of airports</returns>
        Task<IEnumerable<AirportDto>> GetAirportsByCityIdAsync(int cityId);

        /// <summary>
        /// Gets airport by ID
        /// </summary>
        /// <param name="id">Airport ID</param>
        /// <returns>Airport DTO or null</returns>
        Task<AirportDto?> GetAirportAsync(int id);

        /// <summary>
        /// Gets airport by code
        /// </summary>
        /// <param name="code">Airport code</param>
        /// <returns>Airport DTO or null</returns>
        Task<AirportDto?> GetAirportByCodeAsync(string code);

        /// <summary>
        /// Gets airports by country code
        /// </summary>
        /// <param name="countryCode">Country code</param>
        /// <returns>List of airports</returns>
        Task<IEnumerable<AirportDto>> GetAirportsByCountryCodeAsync(string countryCode);
    }
}

