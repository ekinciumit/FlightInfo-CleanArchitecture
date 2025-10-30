using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Interfaces.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetCountriesAsync();
        Task<IEnumerable<CityDto>> GetCountryCitiesAsync(int countryId);
        Task<IEnumerable<AirportDto>> GetCityAirportsAsync(int cityId);
        Task<CountryDto?> GetCountryAsync(int id);
    }
}

