using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Auth
{
    /// <summary>
    /// User login request contract
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}


