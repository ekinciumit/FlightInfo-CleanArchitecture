using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Auth
{
    public class SendSmsCodeRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}



