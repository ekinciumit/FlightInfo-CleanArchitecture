using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _countryService.GetCountriesAsync();
            return Ok(countries);
        }

        // Belirli bir ülkenin şehirlerini getir
        [HttpGet("{countryId}/cities")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCountryCities(int countryId)
        {
            var cities = await _countryService.GetCountryCitiesAsync(countryId);
            return Ok(cities);
        }

        // Belirli bir şehrin havalimanlarını getir
        [HttpGet("cities/{cityId}/airports")]
        public async Task<ActionResult<IEnumerable<AirportDto>>> GetCityAirports(int cityId)
        {
            var airports = await _countryService.GetCityAirportsAsync(cityId);
            return Ok(airports);
        }

        // Belirli bir ülkeyi getir
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countryService.GetCountryAsync(id);
            if (country == null)
                return NotFound();

            return Ok(country);
        }
    }
}


