using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using FlightInfo.Application.Contracts.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FlightInfo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private readonly ILogService _logService;

        public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, Microsoft.Extensions.Configuration.IConfiguration config, ILogService logService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _config = config;
            _logService = logService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string email, string fullName, string password)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email adresi gerekli.");

            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Ad soyad gerekli.");

            if (string.IsNullOrWhiteSpace(password))
                return (false, "Şifre gerekli.");

            if (password.Length < 6)
                return (false, "Şifre en az 6 karakter olmalı.");

            if (!email.Contains("@"))
                return (false, "Geçerli bir email adresi giriniz.");

            if (await _userRepository.EmailExistsAsync(email))
                return (false, "Bu email adresi zaten kayıtlı.");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = email,
                FullName = fullName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Log user registration
            await _logService.LogUserRegistrationAsync(user.Id, user.Email, user.FullName);

            return (true, "User registered successfully.");
        }

    public async Task<(bool Success, string Message, string Token, object? User, bool RequiresTwoFactor, int? UserId)> LoginAsync(string email, string password)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(email))
            return (false, "Email adresi gerekli.", "", null, false, null);

            if (string.IsNullOrWhiteSpace(password))
            return (false, "Şifre gerekli.", "", null, false, null);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return (false, "Email veya şifre hatalı.", "", null, false, null);

            // Silinmiş kullanıcı kontrolü
            if (user.IsDeleted)
                return (false, "Bu hesap silinmiş. Lütfen yönetici ile iletişime geçin.", "", null, false, null);

            // Aktif kullanıcı kontrolü
            if (!user.IsActive)
                return (false, "🔒 Erişiminiz engellendi. Hesabınız yönetici tarafından devre dışı bırakılmıştır. Lütfen yönetici ile iletişime geçin veya destek ekibiyle iletişime geçin.", "", null, false, null);

        // 2FA kontrolü - eğer 2FA aktifse kod iste
        if (user.TwoFactorEnabled && user.TwoFactorType == "Email")
        {
            return (false, "2FA kodu gerekli.", "", null, true, user.Id);
        }

            var token = GenerateJwtToken(user);
            var userInfo = new
            {
                user.Id,
                user.Email,
                user.FullName,
                Role = user.Role.ToString()
            };

            // Log user login
            await _logService.LogUserLoginAsync(user.Id, user.Email, "127.0.0.1");

        return (true, "Giriş başarılı.", token, userInfo, false, null);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("role", user.Role.ToString()) // ASP.NET Core Authorization için
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}

