using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Countries
{
    /// <summary>
    /// Country request contract
    /// </summary>
    public class CountryRequest
    {
        /// <summary>
        /// Country code
        /// </summary>
        [Required(ErrorMessage = "Country code is required")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Country code must be 2-3 characters")]
        [RegularExpression("^[A-Z]{2,3}$", ErrorMessage = "Country code must be uppercase letters")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Country name
        /// </summary>
        [Required(ErrorMessage = "Country name is required")]
        [StringLength(100, ErrorMessage = "Country name must be maximum 100 characters")]
        public string Name { get; set; } = string.Empty;
    }
}


