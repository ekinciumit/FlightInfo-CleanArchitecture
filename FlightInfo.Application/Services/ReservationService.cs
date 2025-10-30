using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlightInfo.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogService _log;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(
            IReservationRepository reservationRepository,
            IFlightRepository flightRepository,
            IUserRepository userRepository,
            ILogService log,
            ICacheService cacheService,
            IUnitOfWork unitOfWork)
        {
            _reservationRepository = reservationRepository;
            _flightRepository = flightRepository;
            _userRepository = userRepository;
            _log = log;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReservationDto> CreateReservationAsync(
            int userId,
            int flightId,
            int flightPriceId,
            string? seatNumber = null)
        {
            // Get user information from database
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new ArgumentException("User not found");

            // Get flight information
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight is null)
                throw new ArgumentException("Flight not found");

            if (flight.Status == "Deleted" || flight.Status == "Cancelled")
                throw new InvalidOperationException("Flight is not available for booking");

            // Check if flight is in the future
            if (flight.DepartureTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot book past flights");

            // Get flight price information
            var flightPrice = flight.FlightPrices.FirstOrDefault(fp => fp.Id == flightPriceId);

            if (flightPrice is null)
                throw new ArgumentException("Flight price not found");

            // Check seat availability
            if (flightPrice.AvailableSeats <= 0)
                throw new InvalidOperationException("No seats available for this class");

            // Check for existing reservation
            var existingReservations = await _reservationRepository.GetByCriteriaAsync(r =>
                r.UserId == userId && r.FlightId == flightId && r.Status == "Confirmed");
            var exists = existingReservations.Any();

            if (exists)
                throw new InvalidOperationException("You already have an active reservation for this flight");

            // Auto-assign seat number if not provided
            if (string.IsNullOrWhiteSpace(seatNumber))
            {
                seatNumber = await GetNextAvailableSeatAsync(flightId, flightPrice.Class);
            }
            else
            {
                // Check if the manually selected seat is already taken
                var existingSeatReservation = await _reservationRepository.GetByCriteriaAsync(r =>
                    r.FlightId == flightId &&
                    r.SeatNumber == seatNumber &&
                    r.Status == "Confirmed");

                if (existingSeatReservation.Any())
                {
                    throw new InvalidOperationException($"Seat {seatNumber} is already taken. Please choose another seat.");
                }
            }

            var reservation = new Reservation
            {
                UserId = userId,
                FlightId = flightId,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                PassengerName = user.FullName,        // User tablosundan
                PassengerEmail = user.Email,          // User tablosundan
                PassengerPhone = user.Phone ?? "",    // User tablosundan (nullable)
                SeatNumber = seatNumber,              // Otomatik atama veya manuel
                Class = flightPrice.Class,            // FlightPrice tablosundan
                TotalPrice = flightPrice.Price,       // FlightPrice tablosundan
                Currency = flightPrice.Currency       // FlightPrice tablosundan
            };

            await _reservationRepository.AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync($"user_reservations_{userId}_all");
            await _cacheService.RemoveAsync($"user_reservations_{userId}_Confirmed");

            // Log reservation creation
            await _log.LogReservationCreateAsync(userId, flightId, flight.FlightNumber, flightPrice.Price);

            return new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                FlightId = reservation.FlightId,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = reservation.Status switch
                {
                    "Pending" => "Pending",
                    "Confirmed" => "Confirmed",
                    "Cancelled" => "Cancelled",
                    "Completed" => "Completed",
                    _ => "Unknown"
                },
                CreatedAt = reservation.CreatedAt,
                PassengerName = reservation.PassengerName,
                PassengerEmail = reservation.PassengerEmail,
                PassengerPhone = reservation.PassengerPhone,
                SeatNumber = reservation.SeatNumber,
                Class = reservation.Class,
                TotalPrice = reservation.TotalPrice,
                Currency = reservation.Currency
            };
        }

        public async Task<ReservationDto?> GetReservationAsync(int id, int userId)
        {
            var reservations = await _reservationRepository.GetByCriteriaAsync(r => r.Id == id && r.UserId == userId);
            var reservation = reservations.FirstOrDefault();

            if (reservation == null)
                return null;

            return new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                FlightId = reservation.FlightId,
                FlightNumber = reservation.Flight.FlightNumber,
                Origin = reservation.Flight.Origin,
                Destination = reservation.Flight.Destination,
                DepartureTime = reservation.Flight.DepartureTime,
                ArrivalTime = reservation.Flight.ArrivalTime,
                Status = reservation.Status switch
                {
                    "Pending" => "Pending",
                    "Confirmed" => "Confirmed",
                    "Cancelled" => "Cancelled",
                    "Completed" => "Completed",
                    _ => "Unknown"
                },
                CreatedAt = reservation.CreatedAt,
                CancelledAt = reservation.CancelledAt,
                PassengerName = reservation.PassengerName,
                PassengerEmail = reservation.PassengerEmail,
                PassengerPhone = reservation.PassengerPhone,
                SeatNumber = reservation.SeatNumber,
                Class = reservation.Class,
                TotalPrice = reservation.TotalPrice,
                Currency = reservation.Currency
            };
        }

        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(int userId, string? status = null)
        {
            // Cache'i geçici olarak devre dışı bırak
            // var cacheKey = $"user_reservations_{userId}_{status ?? "all"}";
            // var cachedReservations = await _cacheService.GetAsync<IEnumerable<ReservationDto>>(cacheKey);
            // if (cachedReservations != null)
            // {
            //     return cachedReservations!;
            // }

            var allReservations = await _reservationRepository.GetByCriteriaAsync(r => r.UserId == userId);

            var filteredReservations = allReservations.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                filteredReservations = filteredReservations.Where(r => r.Status == status);
            }

            var reservations = filteredReservations
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    FlightId = r.FlightId,
                    FlightNumber = r.Flight.FlightNumber,
                    Origin = r.Flight.Origin,
                    Destination = r.Flight.Destination,
                    DepartureTime = r.Flight.DepartureTime,
                    ArrivalTime = r.Flight.ArrivalTime,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    CancelledAt = r.CancelledAt,
                    PassengerName = r.PassengerName,
                    PassengerEmail = r.PassengerEmail,
                    PassengerPhone = r.PassengerPhone,
                    SeatNumber = r.SeatNumber,
                    Class = r.Class,
                    TotalPrice = r.TotalPrice,
                    Currency = r.Currency
                })
                .OrderByDescending(r => r.CreatedAt) // En yeni rezervasyonlar önce
                .ToList();

            // Cache'e kaydetme devre dışı
            // await _cacheService.SetAsync(cacheKey, reservations, TimeSpan.FromMinutes(2));

            return reservations;
        }

        public async Task<bool> CancelReservationAsync(int id, int userId)
        {
            var reservations = await _reservationRepository.GetByCriteriaAsync(r => r.Id == id && r.UserId == userId);
            var reservation = reservations.FirstOrDefault();

            if (reservation == null)
                return false;

            if (reservation.Status == "Cancelled")
                return false;

            reservation.Status = "Cancelled";
            reservation.CancelledAt = DateTime.UtcNow;

            await _reservationRepository.UpdateAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync($"user_reservations_{userId}_all");
            await _cacheService.RemoveAsync($"user_reservations_{userId}_Confirmed");
            await _cacheService.RemoveAsync($"user_reservations_{userId}_Cancelled");

            // Get flight info for logging
            var flight = await _flightRepository.GetByIdAsync(reservation.FlightId);
            var flightNumber = flight?.FlightNumber ?? "Unknown";

            // Log reservation cancellation
            await _log.LogReservationCancelAsync(userId, reservation.FlightId, flightNumber, "User cancelled");

            return true;
        }

        public async Task<ReservationDto> RestoreReservationAsync(int id, int userId)
        {
            var reservations = await _reservationRepository.GetByCriteriaAsync(r => r.Id == id && r.UserId == userId);
            var reservation = reservations.FirstOrDefault();

            if (reservation == null)
                throw new ArgumentException("Reservation not found");

            // Check if reservation is cancelled (handle both string and numeric status)
            var isCancelled = reservation.Status == "Cancelled" ||
                             reservation.Status == "cancelled" ||
                             reservation.Status == "2";

            if (!isCancelled)
                throw new InvalidOperationException($"Only cancelled reservations can be restored. Current status: {reservation.Status}");

            reservation.Status = "Confirmed";
            reservation.CancelledAt = null;

            await _reservationRepository.UpdateAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync($"user_reservations_{userId}_all");
            await _cacheService.RemoveAsync($"user_reservations_{userId}_Confirmed");
            await _cacheService.RemoveAsync($"user_reservations_{userId}_Cancelled");

            // Log
            await _log.LogAsync("Reservation Restored", userId, reservation.FlightId);

            return new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                FlightId = reservation.FlightId,
                FlightNumber = reservation.Flight.FlightNumber,
                Origin = reservation.Flight.Origin,
                Destination = reservation.Flight.Destination,
                DepartureTime = reservation.Flight.DepartureTime,
                ArrivalTime = reservation.Flight.ArrivalTime,
                Status = reservation.Status switch
                {
                    "Pending" => "Pending",
                    "Confirmed" => "Confirmed",
                    "Cancelled" => "Cancelled",
                    "Completed" => "Completed",
                    _ => "Unknown"
                },
                CreatedAt = reservation.CreatedAt,
                CancelledAt = reservation.CancelledAt,
                PassengerName = reservation.PassengerName,
                PassengerEmail = reservation.PassengerEmail,
                PassengerPhone = reservation.PassengerPhone,
                SeatNumber = reservation.SeatNumber,
                Class = reservation.Class,
                TotalPrice = reservation.TotalPrice,
                Currency = reservation.Currency
            };
        }

        public async Task<IEnumerable<object>> GetAllReservationsAsync(string? status = null)
        {
            var allReservations = await _reservationRepository.GetAllAsync();
            var query = allReservations.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var reservations = query
                .Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.FlightId,
                    FlightNumber = r.Flight.FlightNumber,
                    Origin = r.Flight.Origin,
                    Destination = r.Flight.Destination,
                    DepartureTime = r.Flight.DepartureTime,
                    ArrivalTime = r.Flight.ArrivalTime,
                    Status = r.Status,
                    r.CreatedAt,
                    r.CancelledAt,
                    r.PassengerName,
                    r.PassengerEmail,
                    r.PassengerPhone,
                    r.SeatNumber,
                    r.Class,
                    r.TotalPrice,
                    r.Currency,
                    UserName = r.User.FullName,
                    UserEmail = r.User.Email
                })
                .ToList();

            return reservations.Cast<object>();
        }

        private async Task<string> GetNextAvailableSeatAsync(int flightId, string flightClass)
        {
            // Get existing seat numbers for this flight and class
            var allReservations = await _reservationRepository.GetByCriteriaAsync(r => r.FlightId == flightId && r.Class == flightClass && r.Status == "Confirmed");
            var existingSeats = allReservations.Select(r => r.SeatNumber).ToList();

            // Generate seat numbers based on class
            var seatPrefix = flightClass switch
            {
                "Business" => "B",
                "First" => "F",
                _ => "E" // Economy
            };

            // Find next available seat number
            for (int row = 1; row <= 50; row++)
            {
                for (char seat = 'A'; seat <= 'F'; seat++)
                {
                    var seatNumber = $"{seatPrefix}{row}{seat}";
                    if (!existingSeats.Contains(seatNumber))
                    {
                        return seatNumber;
                    }
                }
            }

            // If no seat found, generate a random one
            var randomRow = new Random().Next(1, 51);
            var randomSeat = (char)('A' + new Random().Next(0, 6));
            return $"{seatPrefix}{randomRow}{randomSeat}";
        }
    }
}


