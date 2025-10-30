using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Reservations
{
    /// <summary>
    /// Create reservation request contract - Simplified version
    /// </summary>
    public class CreateReservationRequest
    {
        /// <summary>
        /// Flight ID
        /// </summary>
        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }

        /// <summary>
        /// Flight Price ID (to get exact price and class)
        /// </summary>
        [Required(ErrorMessage = "Flight Price ID is required")]
        public int FlightPriceId { get; set; }

        /// <summary>
        /// Optional: Specific seat number (if empty, auto-assign)
        /// </summary>
        [StringLength(10, ErrorMessage = "Seat number must be maximum 10 characters")]
        public string? SeatNumber { get; set; }
    }
}


