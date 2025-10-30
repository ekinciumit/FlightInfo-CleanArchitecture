namespace FlightInfo.Shared.DTOs
{
    public class FlightSearchCriteria
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? DepartureCity { get; set; }
        public string? ArrivalCity { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? PassengerCount { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }
    }
}

