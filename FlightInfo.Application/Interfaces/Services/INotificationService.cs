using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Interfaces.Services
{
    /// <summary>
    /// Notification service interface
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send email notification
        /// </summary>
        /// <param name="to">Recipient email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <returns>Success status</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="message">SMS message</param>
        /// <returns>Success status</returns>
        Task<bool> SendSmsAsync(string phoneNumber, string message);

        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <returns>Success status</returns>
        Task<bool> SendPushNotificationAsync(int userId, string title, string message);
    }
}

