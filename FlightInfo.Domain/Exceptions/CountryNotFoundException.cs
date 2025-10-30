namespace FlightInfo.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a country is not found
    /// </summary>
    public class CountryNotFoundException : DomainException
    {
        public int CountryId { get; }

        public CountryNotFoundException(int countryId)
            : base($"Country with ID {countryId} was not found")
        {
            CountryId = countryId;
        }

        public CountryNotFoundException(string countryCode)
            : base($"Country with code {countryCode} was not found")
        {
            CountryId = 0;
        }
    }
}


