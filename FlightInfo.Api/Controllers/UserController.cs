using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Contracts.Auth;
using FlightInfo.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // giriş yapmış herkes
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User → sadece Admin
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/User/{id} → sadece Admin
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // PUT: api/User/{id} → sadece Admin
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(user);
        }

        // PUT: api/User/profile → kullanıcı kendi profilini günceller
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized();

            var updated = await _userService.UpdateProfileAsync(userId, request.FullName, request.Phone);
            return Ok(updated);
        }

        // PUT: api/User/{id}/toggle-status → kullanıcı aktif/pasif durumunu değiştir (Admin)
        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(int id, [FromBody] ToggleUserStatusRequest request)
        {
            var success = await _userService.ToggleUserStatusAsync(id, request.IsActive);
            if (!success)
                return NotFound();

            return Ok(new { message = "Kullanıcı durumu başarıyla güncellendi." });
        }

        // DELETE: api/User/{id} → sadece Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }

    // Request DTO for toggle status
    public class ToggleUserStatusRequest
    {
        public bool IsActive { get; set; }
    }
}


