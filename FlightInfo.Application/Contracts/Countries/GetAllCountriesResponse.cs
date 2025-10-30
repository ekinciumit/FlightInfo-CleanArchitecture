namespace FlightInfo.Application.Contracts.Countries
{
    /// <summary>
    /// Get all countries response contract
    /// </summary>
    public class GetAllCountriesResponse
    {
        /// <summary>
        /// Countries list
        /// </summary>
        public List<CountryResponse> Countries { get; set; } = new();

        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Success status
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Country response contract
    /// </summary>
    public class CountryResponse
    {
        /// <summary>
        /// Country ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Country code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Country name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Cities count
        /// </summary>
        public int CitiesCount { get; set; }

        /// <summary>
        /// Cities list
        /// </summary>
        public List<CityResponse>? Cities { get; set; }
    }


    /// <summary>
    /// Airport response contract
    /// </summary>
    public class AirportResponse
    {
        /// <summary>
        /// Airport ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Airport code
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Airport name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Airport full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// City ID
        /// </summary>
        public int CityId { get; set; }
    }
}

