using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        // GET: api/log
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }

        // POST: api/log
        [HttpPost]
        public async Task<IActionResult> CreateLog(CreateLogRequest request)
        {
            var result = await _logService.CreateLogAsync(request);
            return CreatedAtAction(nameof(GetLogs), new { id = result.Id }, result);
        }

        // DELETE: api/log/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var result = await _logService.DeleteLogAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // POST: api/log/test
        [HttpPost("test")]
        public async Task<IActionResult> TestLog()
        {
            await _logService.LogAsync("Test Log - Saat Kontrolü", null, null);
            return Ok(new { message = "Test log oluşturuldu", timestamp = DateTime.Now });
        }
    }
}


