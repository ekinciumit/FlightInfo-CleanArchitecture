using System.Security.Claims;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // giriş yapmış kullanıcılar
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly ILogService _log;

        public FlightController(IFlightService flightService, ILogService log)
        {
            _flightService = flightService;
            _log = log;
        }

        // JWT'den kullanıcı Id'sini al
        private int? GetCurrentUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // GET: api/flight → sadece aktif uçuşları döndür
        [HttpGet]
        [AllowAnonymous] // giriş yapmamış kullanıcılar da görebilir
        public async Task<IActionResult> GetFlights()
        {
            var flights = await _flightService.GetFlightsAsync();
            return Ok(flights);
        }

        // GET: api/flight/search → uçuş arama
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFlights([FromQuery] FlightSearchCriteria criteria)
        {
            var flights = await _flightService.SearchFlightsAsync(criteria);
            return Ok(flights);
        }

        // GET: api/flight/with-prices → uçuşlar + fiyatlar
        [HttpGet("with-prices")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlightsWithPrices()
        {
            var flights = await _flightService.GetFlightsWithPricesAsync();
            return Ok(flights);
        }

        // GET: api/flight/{id}/prices → belirli uçuşun fiyatları
        [HttpGet("{id}/prices")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlightPrices(int id)
        {
            var prices = await _flightService.GetFlightPricesAsync(id);
            return Ok(prices);
        }

        // GET: api/flight/{id}/status → uçuş durumu
        [HttpGet("{id}/status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlightStatus(int id)
        {
            var status = await _flightService.GetFlightStatusAsync(id);
            return Ok(status);
        }

        // GET: api/flight/{id} → belirli uçuş
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlight(int id)
        {
            var flight = await _flightService.GetFlightAsync(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        // POST: api/flight → yeni uçuş oluştur (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFlight([FromBody] FlightDto flightDto)
        {
            var flight = await _flightService.CreateFlightAsync(flightDto);
            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }

        // PUT: api/flight/{id} → uçuş güncelle (Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] FlightDto flightDto)
        {
            var flight = await _flightService.UpdateFlightAsync(id, flightDto);
            return Ok(flight);
        }

        // DELETE: api/flight/{id} → uçuş sil (Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var success = await _flightService.DeleteFlightAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // POST: api/flight/{id}/restore → uçuş geri yükle (Admin)
        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreFlight(int id)
        {
            var flight = await _flightService.RestoreFlightAsync(id);
            return Ok(flight);
        }

        // GET: api/flight/deleted → silinmiş uçuşlar (Admin)
        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedFlights()
        {
            var flights = await _flightService.GetDeletedFlightsAsync();
            return Ok(flights);
        }

        // POST: api/flight/{id}/price → uçuş fiyatı ekle (Admin)
        [HttpPost("{id}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFlightPrice(int id, [FromBody] FlightPriceDto priceDto)
        {
            var success = await _flightService.AddFlightPriceAsync(id, priceDto);
            if (!success)
                return NotFound();

            return Ok();
        }

        // PUT: api/flight/{flightId}/price/{priceId} → uçuş fiyatı güncelle (Admin)
        [HttpPut("{flightId}/price/{priceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFlightPrice(int flightId, int priceId, [FromBody] FlightPriceDto priceDto)
        {
            var success = await _flightService.UpdateFlightPriceAsync(flightId, priceId, priceDto);
            if (!success)
                return NotFound();

            return Ok();
        }

        // DELETE: api/flight/{flightId}/price/{priceId} → uçuş fiyatı sil (Admin)
        [HttpDelete("{flightId}/price/{priceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFlightPrice(int flightId, int priceId)
        {
            var success = await _flightService.DeleteFlightPriceAsync(flightId, priceId);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}


