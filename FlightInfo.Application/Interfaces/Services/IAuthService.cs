using FlightInfo.Domain.Entities;

namespace FlightInfo.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(string email, string fullName, string password);
        Task<(bool Success, string Message, string Token, object? User, bool RequiresTwoFactor, int? UserId)> LoginAsync(string email, string password);
        string GenerateJwtToken(User user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}

