using System.Collections.Generic;
using FlightInfo.Domain.Enums;

namespace FlightInfo.Domain.Entities
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = "";
        public string Origin { get; set; } = "";
        public string Destination { get; set; } = "";
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = FlightStatusEnum.Scheduled;
        public string AircraftType { get; set; } = "";
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<FlightPrice> FlightPrices { get; set; } = new List<FlightPrice>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // Domain methods
        public bool IsAvailable()
        {
            return Status == FlightStatusEnum.Scheduled && DepartureTime > DateTime.UtcNow;
        }

        public bool CanBeBooked()
        {
            return IsAvailable() && FlightPrices.Any(fp => fp.AvailableSeats > 0);
        }

        public void Cancel()
        {
            Status = FlightStatusEnum.Cancelled;
        }
    }
}

