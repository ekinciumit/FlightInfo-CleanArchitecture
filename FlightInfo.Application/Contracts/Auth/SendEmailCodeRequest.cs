using System.ComponentModel.DataAnnotations;

namespace FlightInfo.Application.Contracts.Auth
{
    public class SendEmailCodeRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
    }
}



