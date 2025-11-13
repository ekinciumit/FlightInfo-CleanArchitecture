using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FlightInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITwoFactorService _twoFactorService;

        public AuthController(IAuthService authService, ITwoFactorService twoFactorService)
        {
            _authService = authService;
            _twoFactorService = twoFactorService;
        }

        // ✅ Kullanıcı kayıt (Register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request.Email, request.FullName, request.Password);
            return Ok(new { Message = result.Message });
        }

        // ✅ Kullanıcı giriş (Login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);

            // 2FA gerekiyorsa önce bunu kontrol et
            if (result.RequiresTwoFactor)
            {
                return Ok(new {
                    Message = result.Message,
                    RequiresTwoFactor = true,
                    UserId = result.UserId
                });
            }

            // Hata durumunda BadRequest döndür
            if (!result.Success)
            {
                return BadRequest(new {
                    Message = result.Message,
                    RequiresTwoFactor = false,
                    UserId = result.UserId
                });
            }

            return Ok(new {
                Message = result.Message,
                Token = result.Token,
                User = result.User,
                RequiresTwoFactor = false
            });
        }

        // ✅ 2FA SMS kodu gönder
        [HttpPost("send-sms-code")]
        public async Task<IActionResult> SendSmsCode([FromBody] SendSmsCodeRequest request)
        {
            var result = await _twoFactorService.SendSmsCodeAsync(request.UserId, request.PhoneNumber);
            return Ok(new { Message = result.Message });
        }

        // ✅ 2FA Email kodu gönder
        [HttpPost("send-email-code")]
        public async Task<IActionResult> SendEmailCode([FromBody] SendEmailCodeRequest request)
        {
            var result = await _twoFactorService.SendEmailCodeAsync(request.UserId);
            return Ok(new { Message = result.Message });
        }

        // ✅ 2FA kodu doğrula (Login sırasında)
        [HttpPost("verify-2fa-login")]
        public async Task<IActionResult> VerifyTwoFactorLogin([FromBody] VerifyTwoFactorRequest request)
        {
            var result = await _twoFactorService.VerifyTwoFactorLoginAsync(request.UserId, request.Code);
            return Ok(new {
                Message = result.Message,
                Token = result.Token,
                User = result.User
            });
        }

        // ✅ 2FA'yı etkinleştir
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
        {
            var result = await _twoFactorService.EnableTwoFactorAsync(request.UserId, request.Type);
            return Ok(new { Message = result.Message });
        }

        // ✅ 2FA'yı devre dışı bırak
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
        {
            var result = await _twoFactorService.DisableTwoFactorAsync(request.UserId);
            return Ok(new { Message = result.Message });
        }
    }
}


