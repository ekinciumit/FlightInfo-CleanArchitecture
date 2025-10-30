using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Auth
{
    public class VerifyTwoFactorRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits")]
        public string Code { get; set; } = string.Empty;
    }
}



