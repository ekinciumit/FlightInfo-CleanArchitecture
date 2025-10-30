using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Shared.DTOs;
using FlightInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightInfo.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IAirportRepository airportRepository,
            IUnitOfWork unitOfWork)
        {
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _airportRepository = airportRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            return countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Cities = c.Cities.Select(city => new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    CountryId = city.CountryId,
                    Airports = city.Airports.Select(airport => new AirportDto
                    {
                        Id = airport.Id,
                        Code = airport.Code,
                        Name = airport.Name,
                        CityId = airport.CityId
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<CityDto>> GetCountryCitiesAsync(int countryId)
        {
            var cities = await _cityRepository.GetByCountryIdAsync(countryId);

            return cities.Select(city => new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                CountryId = city.CountryId,
                Airports = city.Airports?.Select(airport => new AirportDto
                {
                    Id = airport.Id,
                    Code = airport.Code,
                    Name = airport.Name,
                    CityId = airport.CityId
                }).ToList() ?? new List<AirportDto>()
            }).ToList();
        }

        public async Task<IEnumerable<AirportDto>> GetCityAirportsAsync(int cityId)
        {
            var airports = await _airportRepository.GetByCityIdAsync(cityId);

            return airports.Select(airport => new AirportDto
            {
                Id = airport.Id,
                Code = airport.Code,
                Name = airport.Name,
                CityId = airport.CityId
            }).ToList();
        }

        public async Task<CountryDto?> GetCountryAsync(int id)
        {
            var allCountries = await _countryRepository.GetAllAsync();
            var countries = allCountries.Where(c => c.Id == id);
            var country = countries.FirstOrDefault();

            if (country == null)
                return null;

            return new CountryDto
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name,
                Cities = country.Cities.Select(city => new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    CountryId = city.CountryId,
                    Airports = city.Airports.Select(airport => new AirportDto
                    {
                        Id = airport.Id,
                        Code = airport.Code,
                        Name = airport.Name,
                        CityId = airport.CityId
                    }).ToList()
                }).ToList()
            };
        }
    }
}

