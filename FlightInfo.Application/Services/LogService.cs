using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FlightInfo.Application.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogService(ILogRepository logRepository, IUserRepository userRepository, IFlightRepository flightRepository, IUnitOfWork unitOfWork)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;
            _flightRepository = flightRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task LogAsync(string action, int? userId, int? flightId = null)
        {
            await LogAsync(action, userId, flightId, null, null);
        }

        public async Task LogAsync(string action, int? userId, int? flightId, object? data, Exception? exception = null)
        {
            // sadece User kontrol et (Flight kontrolünü kaldır)
            var userExists = userId.HasValue && await _userRepository.ExistsAsync(userId.Value);

            var log = new Log
            {
                UserId = userExists ? userId : null,
                FlightId = flightId,   // uçuş silinmiş olabilir → null olabilir
                Action = action,
                Timestamp = DateTime.Now,
                Data = data != null ? JsonSerializer.Serialize(data) : null,
                Exception = exception?.ToString(),
                Level = exception != null ? "Error" : "Info"
            };

            await _logRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task LogExceptionAsync(Exception exception, int? userId, int? flightId = null, object? data = null)
        {
            await LogAsync("Exception", userId, flightId, data, exception);
        }

        public async Task LogAuditAsync(string action, int? userId, object? oldData, object? newData, int? flightId = null)
        {
            var auditData = new
            {
                Action = action,
                OldData = oldData,
                NewData = newData,
                Timestamp = DateTime.Now
            };

            await LogAsync($"Audit_{action}", userId, flightId, auditData);
        }

        public async Task<IEnumerable<LogDto>> GetLogsAsync()
        {
            var logs = await _logRepository.GetAllAsync();
            var logDtos = new List<LogDto>();

            // Get all users and flights in one query for better performance
            var userIds = logs.Where(l => l.UserId.HasValue).Select(l => l.UserId.Value).Distinct().ToList();
            var flightIds = logs.Where(l => l.FlightId.HasValue).Select(l => l.FlightId.Value).Distinct().ToList();

            var users = new Dictionary<int, string>();
            var flights = new Dictionary<int, string>();

            if (userIds.Any())
            {
                var userList = await _userRepository.GetAllAsync();
                users = userList.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u.FullName);
            }

            if (flightIds.Any())
            {
                var flightList = await _flightRepository.GetAllAsync();
                flights = flightList.Where(f => flightIds.Contains(f.Id)).ToDictionary(f => f.Id, f => f.FlightNumber);
            }

            foreach (var log in logs.OrderByDescending(l => l.Timestamp))
            {
                var logDto = new LogDto
                {
                    Id = log.Id,
                    Message = log.Action,
                    Level = log.Level ?? "Info",
                    Timestamp = log.Timestamp,
                    UserId = log.UserId,
                    FlightId = log.FlightId,
                    UserName = null,
                    FlightNumber = null,
                    Details = log.Details,
                    Data = log.Data,
                    Action = log.Action,
                    Exception = log.Exception
                };

                // Get user information if available
                if (log.UserId.HasValue && users.ContainsKey(log.UserId.Value))
                {
                    logDto.UserName = users[log.UserId.Value];
                }

                // Get flight information if available
                if (log.FlightId.HasValue && flights.ContainsKey(log.FlightId.Value))
                {
                    logDto.FlightNumber = flights[log.FlightId.Value];
                }

                logDtos.Add(logDto);
            }

            return logDtos;
        }

        public async Task<LogDto> CreateLogAsync(CreateLogRequest request)
        {
            var log = new Log
            {
                UserId = request.UserId,
                FlightId = request.FlightId,
                Action = request.Message,
                Timestamp = DateTime.Now
            };

            await _logRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return new LogDto
            {
                Id = log.Id,
                Message = log.Action,
                Level = request.Level,
                Timestamp = log.Timestamp,
                UserId = log.UserId,
                FlightId = log.FlightId
            };
        }

        public async Task<bool> DeleteLogAsync(int id)
        {
            var allLogs = await _logRepository.GetAllAsync();
            var logs = allLogs.Where(l => l.Id == id);
            var log = logs.FirstOrDefault();
            if (log == null)
                return false;

            await _logRepository.DeleteAsync(log);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> ClearAllLogsAsync()
        {
            int count = await _logRepository.ClearAllAsync();
            await _unitOfWork.SaveChangesAsync();
            return count;
        }

        // Kullanıcı aktivite logları
        public async Task LogUserLoginAsync(int userId, string email, string ipAddress)
        {
            var loginData = new
            {
                Email = email,
                IPAddress = ipAddress,
                UserAgent = "Web Browser",
                LoginTime = DateTime.Now
            };

            await LogAsync("USER_LOGIN", userId, null, loginData);
        }

        public async Task LogUserLogoutAsync(int userId, string email)
        {
            var logoutData = new
            {
                Email = email,
                LogoutTime = DateTime.Now
            };

            await LogAsync("USER_LOGOUT", userId, null, logoutData);
        }

        public async Task LogUserRegistrationAsync(int userId, string email, string fullName)
        {
            var registrationData = new
            {
                Email = email,
                FullName = fullName,
                RegistrationTime = DateTime.Now
            };

            await LogAsync("USER_REGISTRATION", userId, null, registrationData);
        }

        public async Task LogFlightSearchAsync(int? userId, string searchQuery, string origin, string destination, DateTime? departureDate)
        {
            var searchData = new
            {
                SearchQuery = searchQuery,
                Origin = origin,
                Destination = destination,
                DepartureDate = departureDate,
                SearchTime = DateTime.Now
            };

            await LogAsync("FLIGHT_SEARCH", userId, null, searchData);
        }

        public async Task LogReservationCreateAsync(int userId, int flightId, string flightNumber, decimal totalPrice)
        {
            var reservationData = new
            {
                FlightNumber = flightNumber,
                TotalPrice = totalPrice,
                ReservationTime = DateTime.Now
            };

            await LogAsync("RESERVATION_CREATE", userId, flightId, reservationData);
        }

        public async Task LogReservationCancelAsync(int userId, int flightId, string flightNumber, string reason = null)
        {
            var cancelData = new
            {
                FlightNumber = flightNumber,
                Reason = reason,
                CancelTime = DateTime.Now
            };

            await LogAsync("RESERVATION_CANCEL", userId, flightId, cancelData);
        }

        public async Task LogFlightCreateAsync(int? userId, string flightNumber, string origin, string destination)
        {
            var flightData = new
            {
                FlightNumber = flightNumber,
                Origin = origin,
                Destination = destination,
                CreateTime = DateTime.Now
            };

            await LogAsync("FLIGHT_CREATE", userId, null, flightData);
        }

        public async Task LogFlightUpdateAsync(int? userId, int flightId, string flightNumber, object changes)
        {
            var updateData = new
            {
                FlightNumber = flightNumber,
                Changes = changes,
                UpdateTime = DateTime.Now
            };

            await LogAsync("FLIGHT_UPDATE", userId, flightId, updateData);
        }

        public async Task LogFlightDeleteAsync(int? userId, int flightId, string flightNumber)
        {
            var deleteData = new
            {
                FlightNumber = flightNumber,
                DeleteTime = DateTime.Now
            };

            await LogAsync("FLIGHT_DELETE", userId, flightId, deleteData);
        }

        public async Task LogUserProfileUpdateAsync(int userId, string email, object changes)
        {
            var updateData = new
            {
                Email = email,
                Changes = changes,
                UpdateTime = DateTime.Now
            };

            await LogAsync("USER_PROFILE_UPDATE", userId, null, updateData);
        }

        public async Task LogSystemErrorAsync(string errorMessage, string stackTrace, int? userId = null)
        {
            var errorData = new
            {
                ErrorMessage = errorMessage,
                StackTrace = stackTrace,
                ErrorTime = DateTime.Now
            };

            await LogAsync("SYSTEM_ERROR", userId, null, errorData, new Exception(errorMessage));
        }
    }
}

