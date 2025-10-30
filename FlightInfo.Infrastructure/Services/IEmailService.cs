namespace FlightInfo.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(string to, string cc, string subject, string body, bool isHtml = true);
    }
}


