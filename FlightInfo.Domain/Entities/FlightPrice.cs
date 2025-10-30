namespace FlightInfo.Domain.Entities
{
    public class FlightPrice
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string Class { get; set; } = "Economy"; // Economy, Business, First
        public decimal Price { get; set; }
        public string Currency { get; set; } = "TRY";
        public int AvailableSeats { get; set; } = 0;
        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Flight Flight { get; set; } = null!;
    }
}


