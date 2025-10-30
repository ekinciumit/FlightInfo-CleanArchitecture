using FlightInfo.Domain.Events;

namespace FlightInfo.Domain.Events
{
    /// <summary>
    /// Event raised when a country is created
    /// </summary>
    public class CountryCreatedEvent : IDomainEvent
    {
        public int CountryId { get; }
        public string CountryName { get; }
        public string CountryCode { get; }
        public DateTime OccurredOn { get; }

        public CountryCreatedEvent(int countryId, string countryName, string countryCode)
        {
            CountryId = countryId;
            CountryName = countryName;
            CountryCode = countryCode;
            OccurredOn = DateTime.UtcNow;
        }
    }
}


