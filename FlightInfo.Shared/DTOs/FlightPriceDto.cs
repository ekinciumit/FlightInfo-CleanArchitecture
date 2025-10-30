namespace FlightInfo.Shared.DTOs
{
    public class FlightPriceDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string Class { get; set; } = ""; // Economy, Business, First
        public decimal Price { get; set; }
        public string Currency { get; set; } = "TRY";
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

