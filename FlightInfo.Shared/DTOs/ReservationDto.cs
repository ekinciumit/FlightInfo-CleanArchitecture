namespace FlightInfo.Shared.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }

        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        // Yolcu bilgileri
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string Class { get; set; } = "Economy";
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "TRY";
    }
}


