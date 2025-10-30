using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightInfo.Application.Services
{
    /// <summary>
    /// City service implementation
    /// </summary>
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<CityDto>> GetCitiesAsync()
        {
            var cities = await _cityRepository.GetAllAsync();
            return cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                CountryId = c.CountryId
            });
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByCountryIdAsync(int countryId)
        {
            var cities = await _cityRepository.GetByCountryIdAsync(countryId);
            return cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                CountryId = c.CountryId
            });
        }

        public async Task<CityDto?> GetCityAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null) return null;

            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Code = city.Code,
                CountryId = city.CountryId
            };
        }

        public async Task<CityDto?> GetCityByCodeAsync(string code)
        {
            var city = await _cityRepository.GetByCodeAsync(code);
            if (city == null) return null;

            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Code = city.Code,
                CountryId = city.CountryId
            };
        }
    }
}

