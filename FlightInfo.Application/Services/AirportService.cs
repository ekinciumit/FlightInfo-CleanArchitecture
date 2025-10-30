using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightInfo.Application.Services
{
    /// <summary>
    /// Airport service implementation
    /// </summary>
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _airportRepository;

        public AirportService(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository;
        }

        public async Task<IEnumerable<AirportDto>> GetAirportsAsync()
        {
            var airports = await _airportRepository.GetAllAsync();
            return airports.Select(a => new AirportDto
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                FullName = a.FullName,
                CityId = a.CityId
            });
        }

        public async Task<IEnumerable<AirportDto>> GetAirportsByCityIdAsync(int cityId)
        {
            var airports = await _airportRepository.GetByCityIdAsync(cityId);
            return airports.Select(a => new AirportDto
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                FullName = a.FullName,
                CityId = a.CityId
            });
        }

        public async Task<AirportDto?> GetAirportAsync(int id)
        {
            var airport = await _airportRepository.GetByIdAsync(id);
            if (airport == null) return null;

            return new AirportDto
            {
                Id = airport.Id,
                Code = airport.Code,
                Name = airport.Name,
                FullName = airport.FullName,
                CityId = airport.CityId
            };
        }

        public async Task<AirportDto?> GetAirportByCodeAsync(string code)
        {
            var airport = await _airportRepository.GetByCodeAsync(code);
            if (airport == null) return null;

            return new AirportDto
            {
                Id = airport.Id,
                Code = airport.Code,
                Name = airport.Name,
                FullName = airport.FullName,
                CityId = airport.CityId
            };
        }

        public async Task<IEnumerable<AirportDto>> GetAirportsByCountryCodeAsync(string countryCode)
        {
            var airports = await _airportRepository.GetByCountryCodeAsync(countryCode);
            return airports.Select(a => new AirportDto
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                FullName = a.FullName,
                CityId = a.CityId
            });
        }
    }
}

