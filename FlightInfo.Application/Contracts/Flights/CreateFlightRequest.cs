using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Flights
{
    /// <summary>
    /// Create flight request contract
    /// </summary>
    public class CreateFlightRequest
    {
        /// <summary>
        /// Flight number
        /// </summary>
        [Required(ErrorMessage = "Flight number is required")]
        [StringLength(10, ErrorMessage = "Flight number must be maximum 10 characters")]
        public string FlightNumber { get; set; } = string.Empty;

        /// <summary>
        /// Origin airport
        /// </summary>
        [Required(ErrorMessage = "Origin is required")]
        [StringLength(50, ErrorMessage = "Origin must be maximum 50 characters")]
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Destination airport
        /// </summary>
        [Required(ErrorMessage = "Destination is required")]
        [StringLength(50, ErrorMessage = "Destination must be maximum 50 characters")]
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Departure time
        /// </summary>
        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Arrival time
        /// </summary>
        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Flight status
        /// </summary>
        [StringLength(20, ErrorMessage = "Status must be maximum 20 characters")]
        public string Status { get; set; } = "Scheduled";
    }
}


