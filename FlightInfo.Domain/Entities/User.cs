using System.ComponentModel.DataAnnotations;
using FlightInfo.Domain.Enums;

namespace FlightInfo.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Role { get; set; } = UserRoleEnum.User;
        public string? Phone { get; set; } // Optional phone number
        public bool PhoneVerified { get; set; } = false; // Telefon doğrulandı mı?
        public bool TwoFactorEnabled { get; set; } = false; // 2FA aktif mi?
        public string TwoFactorType { get; set; } = "None"; // "SMS", "Email", "None"
        public DateTime? TwoFactorSetupAt { get; set; } // 2FA kurulum tarihi
        public DateTime? LastTwoFactorAt { get; set; } // Son 2FA kullanımı
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}


