using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Auth
{
    public class EnableTwoFactorRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [RegularExpression("^(SMS|Email)$", ErrorMessage = "Type must be either SMS or Email")]
        public string Type { get; set; } = string.Empty;
    }
}



