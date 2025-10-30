using FlightInfo.Domain.Events;

namespace FlightInfo.Domain.Events
{
    public class FlightBookedEvent : IDomainEvent
    {
        public int FlightId { get; }
        public int UserId { get; }
        public int ReservationId { get; }
        public DateTime OccurredOn { get; }

        public FlightBookedEvent(int flightId, int userId, int reservationId)
        {
            FlightId = flightId;
            UserId = userId;
            ReservationId = reservationId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}

