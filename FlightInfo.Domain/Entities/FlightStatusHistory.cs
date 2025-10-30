namespace FlightInfo.Domain.Entities
{
    public class FlightStatusHistory
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string Status { get; set; } = "";
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? ChangedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public Flight Flight { get; set; } = null!;
    }
}

