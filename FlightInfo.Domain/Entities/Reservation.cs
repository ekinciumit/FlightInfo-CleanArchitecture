using FlightInfo.Domain.Enums;

namespace FlightInfo.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public string PassengerName { get; set; } = "";
        public string PassengerEmail { get; set; } = "";
        public string PassengerPhone { get; set; } = "";
        public string SeatNumber { get; set; } = "";
        public string Class { get; set; } = "Economy"; // Economy, Business, First
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Status { get; set; } = ReservationStatusEnum.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Flight Flight { get; set; } = null!;
    }
}

