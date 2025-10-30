using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Interfaces.Services
{
    public interface ILogService
    {
        Task LogAsync(string action, int? userId, int? flightId = null);
        Task LogAsync(string action, int? userId, int? flightId, object? data, Exception? exception = null);
        Task LogExceptionAsync(Exception exception, int? userId, int? flightId = null, object? data = null);
        Task LogAuditAsync(string action, int? userId, object? oldData, object? newData, int? flightId = null);
        Task<IEnumerable<LogDto>> GetLogsAsync();
        Task<LogDto> CreateLogAsync(CreateLogRequest request);
        Task<bool> DeleteLogAsync(int id);

        // Kullanıcı aktivite logları
        Task LogUserLoginAsync(int userId, string email, string ipAddress);
        Task LogUserLogoutAsync(int userId, string email);
        Task LogUserRegistrationAsync(int userId, string email, string fullName);
        Task LogFlightSearchAsync(int? userId, string searchQuery, string origin, string destination, DateTime? departureDate);
        Task LogReservationCreateAsync(int userId, int flightId, string flightNumber, decimal totalPrice);
        Task LogReservationCancelAsync(int userId, int flightId, string flightNumber, string reason = null);
        Task LogFlightCreateAsync(int? userId, string flightNumber, string origin, string destination);
        Task LogFlightUpdateAsync(int? userId, int flightId, string flightNumber, object changes);
        Task LogFlightDeleteAsync(int? userId, int flightId, string flightNumber);
        Task LogUserProfileUpdateAsync(int userId, string email, object changes);
        Task LogSystemErrorAsync(string errorMessage, string stackTrace, int? userId = null);
    }
}

