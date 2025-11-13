using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace FlightInfo.Application.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITwoFactorCodeRepository _twoFactorCodeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;

        public TwoFactorService(
            IUserRepository userRepository,
            ITwoFactorCodeRepository twoFactorCodeRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IAuthService authService,
            ILogService logService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _twoFactorCodeRepository = twoFactorCodeRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _authService = authService;
            _logService = logService;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> SendSmsCodeAsync(int userId, string phoneNumber)
        {
            return (false, "SMS 2FA devre dışı bırakıldı. Lütfen Email 2FA kullanın.");
        }

        public async Task<(bool Success, string Message)> SendEmailCodeAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return (false, "Kullanıcı bulunamadı.");

                // Gerçek random kod üret
                var random = new Random();
                var code = random.Next(100000, 999999).ToString();

                // Eski kodları temizle (hem süresi dolmuş hem de aktif olanları)
                await CleanAllUserCodesAsync(userId);

                // Yeni kodu veritabanına kaydet
                var twoFactorCode = new TwoFactorCode
                {
                    UserId = userId,
                    Code = code,
                    Type = "Email",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                };

                await _twoFactorCodeRepository.AddAsync(twoFactorCode);
                await _unitOfWork.SaveChangesAsync();

                // Gerçek email gönder
                try
                {
                    await _notificationService.SendEmailAsync(
                        user.Email,
                        "FlightInfo Güvenlik Kodu",
                        $"Güvenlik kodunuz: {code}\n\nBu kod 5 dakika geçerlidir.\n\nFlightInfo Ekibi"
                    );
                    return (true, "Email kodu gönderildi");
                }
                catch (Exception emailEx)
                {
                    await _logService.LogExceptionAsync(emailEx, userId);
                    return (true, $"Email gönderilemedi ama kod: {code}");
                }
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, userId);
                return (false, "Email gönderilirken bir hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> VerifySmsCodeAsync(int userId, string code)
        {
            return await VerifyCodeAsync(userId, code, "SMS");
        }

        public async Task<(bool Success, string Message)> VerifyEmailCodeAsync(int userId, string code)
        {
            return await VerifyCodeAsync(userId, code, "Email");
        }

        private async Task<(bool Success, string Message)> VerifyCodeAsync(int userId, string code, string type)
        {
            try
            {
                // Debug log - Console'a yazdır
                Console.WriteLine($"🔍 DEBUG: Verifying code {code} for user {userId} type {type}");

                // En son kodu al (sadece kullanılmamış ve geçerli olanlar)
                var userCodes = await _twoFactorCodeRepository.GetUserCodesAsync(userId);

                // Tüm kodları debug et
                Console.WriteLine($"🔍 DEBUG: All user codes count: {userCodes.Count}");
                foreach (var c in userCodes.Take(5)) // İlk 5 kodu göster
                {
                    Console.WriteLine($"  - Code: {c.Code}, Type: {c.Type}, IsUsed: {c.IsUsed}, CreatedAt: {c.CreatedAt}, ExpiresAt: {c.ExpiresAt}");
                }

                var latestCode = userCodes
                    .Where(x => x.Type == type && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                // Debug log - Console'a yazdır
                Console.WriteLine($"🔍 DEBUG: LatestCode found: {latestCode != null}, Code: {latestCode?.Code}, IsUsed: {latestCode?.IsUsed}, ExpiresAt: {latestCode?.ExpiresAt}");

                // Sadece en son kod ile karşılaştır
                if (latestCode == null || latestCode.Code != code)
                {
                    Console.WriteLine($"❌ DEBUG: Code validation FAILED - latest code mismatch or not found");
                    Console.WriteLine($"❌ DEBUG: Expected: {latestCode?.Code}, Got: {code}");
                    return (false, "Geçersiz veya süresi dolmuş kod.");
                }

                // Kodu kullanılmış olarak işaretle ve sil
                latestCode.IsUsed = true;
                await _twoFactorCodeRepository.DeleteAsync(latestCode);
                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"✅ DEBUG: Code validation SUCCESS - latest code verified and deleted");
                return (true, "Kod doğrulandı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Exception in VerifyCodeAsync: {ex.Message}");
                await _logService.LogExceptionAsync(ex, userId);
                return (false, "Kod doğrulanırken bir hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> EnableTwoFactorAsync(int userId, string type)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return (false, "Kullanıcı bulunamadı.");

                if (type != "Email")
                    return (false, "Geçersiz 2FA tipi. Sadece Email desteklenmektedir.");

                user.TwoFactorEnabled = true;
                user.TwoFactorType = type;
                user.TwoFactorSetupAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                return (true, $"{type} ile 2FA başarıyla etkinleştirildi.");
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, userId);
                return (false, "2FA etkinleştirilirken bir hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> DisableTwoFactorAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return (false, "Kullanıcı bulunamadı.");

                user.TwoFactorEnabled = false;
                user.TwoFactorType = "None";
                user.TwoFactorSetupAt = null;

                // Kullanıcının tüm 2FA kodlarını temizle
                await CleanExpiredCodesAsync(userId);

                await _unitOfWork.SaveChangesAsync();

                return (true, "2FA başarıyla devre dışı bırakıldı.");
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, userId);
                return (false, "2FA devre dışı bırakılırken bir hata oluştu.");
            }
        }

        public async Task<bool> IsTwoFactorEnabledAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.TwoFactorEnabled ?? false;
        }

        public async Task<string?> GetTwoFactorTypeAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.TwoFactorType;
        }

        public async Task<(bool Success, string Message, string Token, object? User)> VerifyTwoFactorLoginAsync(int userId, string code)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return (false, "Kullanıcı bulunamadı.", "", null);

                // Silinmiş kullanıcı kontrolü
                if (user.IsDeleted)
                    return (false, "Bu hesap silinmiş. Lütfen yönetici ile iletişime geçin.", "", null);

                // Aktif kullanıcı kontrolü
                if (!user.IsActive)
                    return (false, "🔒 Erişiminiz engellendi. Hesabınız yönetici tarafından devre dışı bırakılmıştır. Lütfen yönetici ile iletişime geçin veya destek ekibiyle iletişime geçin.", "", null);

                var verifyResult = await VerifyCodeAsync(userId, code, "Email");
                if (!verifyResult.Success)
                    return (false, verifyResult.Message, "", null);

                // JWT token oluştur
                var token = _authService.GenerateJwtToken(user);
                var userInfo = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    Role = user.Role.ToString()
                };

                // Son 2FA kullanımını güncelle
                user.LastTwoFactorAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                // Log
                await _logService.LogUserLoginAsync(user.Id, user.Email, "127.0.0.1");

                return (true, "Giriş başarılı.", token, userInfo);
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, userId);
                return (false, "Giriş doğrulanırken bir hata oluştu.", "", null);
            }
        }

        private string GenerateRandomCode(int length)
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basit telefon numarası validasyonu
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            // + ile başlamalı ve en az 10 karakter olmalı
            return phoneNumber.StartsWith("+") && phoneNumber.Length >= 10;
        }

        private string CreateEmailTemplate(string fullName, string code)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 10px; text-align: center;'>
                    <h1 style='margin: 0; font-size: 28px;'>FlightInfo</h1>
                    <p style='margin: 10px 0 0 0; opacity: 0.9;'>Güvenlik Doğrulama</p>
                </div>

                <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
                    <h2 style='color: #333; margin-top: 0;'>Merhaba {fullName},</h2>
                    <p style='color: #666; font-size: 16px; line-height: 1.6;'>
                        FlightInfo hesabınız için güvenlik kodunuz:
                    </p>

                    <div style='background: white; border: 2px dashed #667eea; border-radius: 8px; padding: 20px; margin: 20px 0; text-align: center;'>
                        <h1 style='color: #667eea; font-size: 36px; margin: 0; letter-spacing: 5px;'>{code}</h1>
                    </div>

                    <p style='color: #666; font-size: 14px;'>
                        Bu kod 5 dakika geçerlidir. Eğer bu işlemi siz yapmadıysanız,
                        lütfen hemen bizimle iletişime geçin.
                    </p>

                    <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd;'>
                        <p style='color: #999; font-size: 12px; margin: 0;'>
                            İyi günler,<br>
                            <strong>FlightInfo Ekibi</strong>
                        </p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private async Task CleanExpiredCodesAsync(int userId)
        {
            // Kullanıcının süresi dolmuş kodlarını temizle
            var expiredCodes = await _twoFactorCodeRepository
                .GetExpiredCodesAsync(userId);

            foreach (var code in expiredCodes)
            {
                await _twoFactorCodeRepository.DeleteAsync(code);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task CleanAllUserCodesAsync(int userId)
        {
            try
            {
                // Kullanıcının tüm kodlarını sil (aktif ve süresi dolmuş)
                var allCodes = await _twoFactorCodeRepository.GetUserCodesAsync(userId);
                foreach (var code in allCodes)
                {
                    await _twoFactorCodeRepository.DeleteAsync(code);
                }
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionAsync(ex, userId);
            }
        }
    }
}

