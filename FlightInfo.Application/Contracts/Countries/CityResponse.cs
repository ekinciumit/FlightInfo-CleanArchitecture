namespace FlightInfo.Application.Contracts.Countries
{
    /// <summary>
    /// City response contract
    /// </summary>
    public class CityResponse
    {
        /// <summary>
        /// City ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// City name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// City code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Country ID
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>
        /// Country code
        /// </summary>
        public string CountryCode { get; set; } = string.Empty;
    }
}

