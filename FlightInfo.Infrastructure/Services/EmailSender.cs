using FlightInfo.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace FlightInfo.Infrastructure.Services
{
    /// <summary>
    /// Email sender service implementation
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _smtpHost = configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = configuration["EmailSettings:Username"] ?? "";
            _smtpPassword = configuration["EmailSettings:Password"] ?? "";
            _enableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"] ?? "true");
        }

        /// <summary>
        /// Send email
        /// </summary>
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpHost, _smtpPort);
                client.EnableSsl = _enableSsl;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                using var message = new MailMessage();
                message.From = new MailAddress(_smtpUsername);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return false;
            }
        }

        /// <summary>
        /// Send email with template
        /// </summary>
        public async Task<bool> SendTemplateEmailAsync(string to, string templateName, object templateData)
        {
            try
            {
                // Template processing logic would go here
                var subject = $"Template: {templateName}";
                var body = $"Template: {templateName} with data: {templateData}";

                return await SendEmailAsync(to, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template email to {Email}", to);
                return false;
            }
        }

        /// <summary>
        /// Send bulk email
        /// </summary>
        public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true)
        {
            try
            {
                var tasks = recipients.Select(recipient => SendEmailAsync(recipient, subject, body));
                var results = await Task.WhenAll(tasks);

                var successCount = results.Count(r => r);
                _logger.LogInformation("Bulk email sent: {SuccessCount}/{TotalCount} successful", successCount, recipients.Count);

                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send bulk email to {RecipientCount} recipients", recipients.Count);
                return false;
            }
        }
    }
}
