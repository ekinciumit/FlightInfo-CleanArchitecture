namespace FlightInfo.Application.Contracts.Reservations
{
    /// <summary>
    /// Reservation response contract
    /// </summary>
    public class ReservationResponse
    {
        /// <summary>
        /// Reservation ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Flight ID
        /// </summary>
        public int FlightId { get; set; }

        /// <summary>
        /// Passenger name
        /// </summary>
        public string PassengerName { get; set; } = string.Empty;

        /// <summary>
        /// Passenger email
        /// </summary>
        public string PassengerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Passenger phone
        /// </summary>
        public string PassengerPhone { get; set; } = string.Empty;

        /// <summary>
        /// Seat number
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Flight class
        /// </summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// Total price
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Reservation status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Created at
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Flight information
        /// </summary>
        public FlightInfo? Flight { get; set; }
    }

    /// <summary>
    /// Flight information for reservation
    /// </summary>
    public class FlightInfo
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
        /// Origin
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Destination
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
    }
}


