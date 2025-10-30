namespace FlightInfo.Application.Contracts.Flights
{
    /// <summary>
    /// Flight response contract
    /// </summary>
    public class FlightResponse
    {
        /// <summary>
        /// Flight ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Flight number
        /// </summary>
        public string FlightNumber { get; set; } = string.Empty;

        /// <summary>
        /// Origin airport
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Destination airport
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Departure time
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Arrival time
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Flight status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Flight prices
        /// </summary>
        public List<FlightPriceResponse>? FlightPrices { get; set; }
    }

    /// <summary>
    /// Flight price response contract
    /// </summary>
    public class FlightPriceResponse
    {
        /// <summary>
        /// Price ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Flight ID
        /// </summary>
        public int FlightId { get; set; }

        /// <summary>
        /// Class name
        /// </summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// Price amount
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Available seats
        /// </summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Created at
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}


