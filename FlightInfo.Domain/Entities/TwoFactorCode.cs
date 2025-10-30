using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Domain.Entities
{
    public class TwoFactorCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = "";
        public string Type { get; set; } = ""; // "SMS" veya "Email"
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? PhoneNumber { get; set; } // SMS için
        public string? Email { get; set; } // Email için

        // Navigation property
        public User User { get; set; } = null!;
    }
}



