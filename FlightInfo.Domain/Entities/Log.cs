namespace FlightInfo.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Action { get; set; } = ""; // Log aksiyonu (örn: "User Login", "Flight Created")
        public string Message { get; set; } = "";
        public string Level { get; set; } = "Information"; // Information, Warning, Error
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; } // Nullable - sistem logları için
        public int? FlightId { get; set; } // Nullable - uçuş ile ilgili loglar için
        public string? Details { get; set; } // JSON formatında ek detaylar
        public string? Data { get; set; } // JSON formatında ek veri
        public string? Exception { get; set; } // Exception detayları

        // Navigation properties
        public User? User { get; set; }
        public Flight? Flight { get; set; }
    }
}

