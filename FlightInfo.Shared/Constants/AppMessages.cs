namespace FlightInfo.Shared.Constants
{
    /// <summary>
    /// Application messages constants
    /// </summary>
    public static class AppMessages
    {
        // Authentication Messages
        public const string LOGIN_SUCCESS = "Giriş başarılı";
        public const string LOGIN_FAILED = "Email veya şifre hatalı";
        public const string REGISTER_SUCCESS = "Kayıt başarılı";
        public const string REGISTER_FAILED = "Kayıt başarısız";
        public const string EMAIL_ALREADY_EXISTS = "Bu email adresi zaten kayıtlı";
        public const string INVALID_EMAIL = "Geçerli bir email adresi giriniz";
        public const string PASSWORD_REQUIRED = "Şifre gerekli";
        public const string PASSWORD_TOO_SHORT = "Şifre en az 6 karakter olmalı";
        public const string FULL_NAME_REQUIRED = "Ad soyad gerekli";
        public const string EMAIL_REQUIRED = "Email adresi gerekli";

        // Flight Messages
        public const string FLIGHT_NOT_FOUND = "Uçuş bulunamadı";
        public const string FLIGHT_NOT_AVAILABLE = "Uçuş müsait değil";
        public const string FLIGHT_CANCELLED = "Uçuş iptal edildi";
        public const string FLIGHT_DELAYED = "Uçuş gecikti";
        public const string FLIGHT_COMPLETED = "Uçuş tamamlandı";
        public const string FLIGHT_CREATED = "Uçuş oluşturuldu";
        public const string FLIGHT_UPDATED = "Uçuş güncellendi";
        public const string FLIGHT_DELETED = "Uçuş silindi";

        // Reservation Messages
        public const string RESERVATION_CREATED = "Rezervasyon oluşturuldu";
        public const string RESERVATION_UPDATED = "Rezervasyon güncellendi";
        public const string RESERVATION_CANCELLED = "Rezervasyon iptal edildi";
        public const string RESERVATION_RESTORED = "Rezervasyon geri alındı";
        public const string RESERVATION_NOT_FOUND = "Rezervasyon bulunamadı";
        public const string RESERVATION_ALREADY_EXISTS = "Bu uçuş için zaten rezervasyonunuz var";
        public const string CANNOT_BOOK_PAST_FLIGHT = "Geçmiş uçuşlar için rezervasyon yapılamaz";
        public const string INSUFFICIENT_SEATS = "Yeterli koltuk yok";

        // User Messages
        public const string USER_NOT_FOUND = "Kullanıcı bulunamadı";
        public const string USER_CREATED = "Kullanıcı oluşturuldu";
        public const string USER_UPDATED = "Kullanıcı güncellendi";
        public const string USER_DELETED = "Kullanıcı silindi";
        public const string USER_ALREADY_EXISTS = "Kullanıcı zaten mevcut";

        // General Messages
        public const string SUCCESS = "İşlem başarılı";
        public const string ERROR = "Bir hata oluştu";
        public const string VALIDATION_ERROR = "Doğrulama hatası";
        public const string UNAUTHORIZED = "Yetkisiz erişim";
        public const string FORBIDDEN = "Erişim reddedildi";
        public const string NOT_FOUND = "Kayıt bulunamadı";
        public const string BAD_REQUEST = "Geçersiz istek";
        public const string INTERNAL_SERVER_ERROR = "Sunucu hatası";
    }
}


