using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Flights
{
    /// <summary>
    /// Update flight request contract
    /// </summary>
    public class UpdateFlightRequest
    {
        /// <summary>
        /// Flight ID
        /// </summary>
        [Required(ErrorMessage = "Flight ID is required")]
        public int Id { get; set; }

        /// <summary>
        /// Flight number
        /// </summary>
        [StringLength(10, ErrorMessage = "Flight number must be maximum 10 characters")]
        public string? FlightNumber { get; set; }

        /// <summary>
        /// Origin airport
        /// </summary>
        [StringLength(50, ErrorMessage = "Origin must be maximum 50 characters")]
        public string? Origin { get; set; }

        /// <summary>
        /// Destination airport
        /// </summary>
        [StringLength(50, ErrorMessage = "Destination must be maximum 50 characters")]
        public string? Destination { get; set; }

        /// <summary>
        /// Departure time
        /// </summary>
        public DateTime? DepartureTime { get; set; }

        /// <summary>
        /// Arrival time
        /// </summary>
        public DateTime? ArrivalTime { get; set; }

        /// <summary>
        /// Flight status
        /// </summary>
        [StringLength(20, ErrorMessage = "Status must be maximum 20 characters")]
        public string? Status { get; set; }
    }
}


