namespace FlightInfo.Application.Interfaces.Services
{
    public interface ITwoFactorService
    {
        // SMS 2FA
        Task<(bool Success, string Message)> SendSmsCodeAsync(int userId, string phoneNumber);
        Task<(bool Success, string Message)> VerifySmsCodeAsync(int userId, string code);

        // Email 2FA
        Task<(bool Success, string Message)> SendEmailCodeAsync(int userId);
        Task<(bool Success, string Message)> VerifyEmailCodeAsync(int userId, string code);

        // Genel işlemler
        Task<(bool Success, string Message)> EnableTwoFactorAsync(int userId, string type);
        Task<(bool Success, string Message)> DisableTwoFactorAsync(int userId);
        Task<bool> IsTwoFactorEnabledAsync(int userId);
        Task<string?> GetTwoFactorTypeAsync(int userId);

        // Login sırasında 2FA doğrulama
        Task<(bool Success, string Message, string Token, object? User)> VerifyTwoFactorLoginAsync(int userId, string code);
    }
}



