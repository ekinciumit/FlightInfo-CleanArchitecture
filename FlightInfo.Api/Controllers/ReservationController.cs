using System.Security.Claims;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Shared.DTOs;
using FlightInfo.Application.Contracts.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private int? GetCurrentUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }


        // POST: api/Reservation → Bilet satın al (giriş yapmış kullanıcılar)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReservationRequest req)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var reservation = await _reservationService.CreateReservationAsync(
                userId.Value,
                req.FlightId,
                req.FlightPriceId,
                req.SeatNumber
            );

            return CreatedAtAction(nameof(Get), new { id = reservation.Id }, reservation);
        }

        // GET: api/Reservation/{id} → Belirli rezervasyon
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var reservation = await _reservationService.GetReservationAsync(id, userId.Value);
            if (reservation is null) return NotFound();

            return Ok(reservation);
        }

        // GET: api/Reservation → Kullanıcının rezervasyonları
        [HttpGet]
        public async Task<IActionResult> GetUserReservations([FromQuery] string? status = null)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var reservations = await _reservationService.GetUserReservationsAsync(userId.Value, status);
            return Ok(reservations);
        }

        // DELETE: api/Reservation/{id} → Rezervasyon iptal et
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var success = await _reservationService.CancelReservationAsync(id, userId.Value);
            if (!success) return NotFound();

            return NoContent();
        }

        // POST: api/Reservation/{id}/restore → Rezervasyon geri yükle
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var reservation = await _reservationService.RestoreReservationAsync(id, userId.Value);
            return Ok(reservation);
        }

        // GET: api/Reservation/admin → Tüm rezervasyonlar (Admin)
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllReservations([FromQuery] string? status = null)
        {
            var reservations = await _reservationService.GetAllReservationsAsync(status);
            return Ok(reservations);
        }
    }
}


