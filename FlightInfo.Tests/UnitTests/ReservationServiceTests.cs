using FluentAssertions;
using FlightInfo.Application.Services;
using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using FlightInfo.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System.Linq.Expressions;

namespace FlightInfo.Tests.UnitTests
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IFlightRepository> _flightRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<FlightInfo.Application.Interfaces.Services.ILogService> _logServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _logServiceMock = new Mock<ILogService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _reservationService = new ReservationService(
                _reservationRepositoryMock.Object,
                _flightRepositoryMock.Object,
                _userRepositoryMock.Object,
                _logServiceMock.Object,
                _cacheServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateReservationAsync_WithValidData_ShouldReturnReservationDto()
        {
            var userId = 1;
            var flightId = 1;
            var flightPriceId = 1;
            var seatNumber = "A1";

            var user = new User
            {
                Id = userId,
                FullName = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890"
            };

            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "TK001",
                Origin = "IST",
                Destination = "LHR",
                DepartureTime = DateTime.UtcNow.AddDays(1),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(3),
                Status = "Active",
                FlightPrices = new List<FlightPrice>
                {
                    new FlightPrice
                    {
                        Id = flightPriceId,
                        Class = "Economy",
                        Price = 100.0m,
                        Currency = "USD",
                        AvailableSeats = 10
                    }
                }
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _flightRepositoryMock.Setup(r => r.GetByIdAsync(flightId))
                .ReturnsAsync(flight);

            _reservationRepositoryMock.Setup(r => r.GetByCriteriaAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
                .ReturnsAsync(new List<Reservation>());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _reservationService.CreateReservationAsync(
                userId, flightId, flightPriceId, seatNumber);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.FlightId.Should().Be(flightId);
            result.PassengerName.Should().Be(user.FullName);
            result.PassengerEmail.Should().Be(user.Email);
            result.PassengerPhone.Should().Be(user.Phone);
            result.SeatNumber.Should().Be(seatNumber);
            result.Class.Should().Be("Economy");
        }

        [Fact]
        public async Task CreateReservationAsync_WithNonExistentUser_ShouldThrowArgumentException()
        {
            var userId = 999;
            var flightId = 1;
            var flightPriceId = 1;
            var seatNumber = "A1";

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _reservationService.CreateReservationAsync(
                    userId, flightId, flightPriceId, seatNumber));
        }

        [Fact]
        public async Task CreateReservationAsync_WithNonExistentFlight_ShouldThrowArgumentException()
        {
            var userId = 1;
            var flightId = 999;
            var flightPriceId = 1;
            var seatNumber = "A1";

            var user = new User
            {
                Id = userId,
                FullName = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _flightRepositoryMock.Setup(r => r.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _reservationService.CreateReservationAsync(
                    userId, flightId, flightPriceId, seatNumber));
        }

        [Fact]
        public async Task CancelReservationAsync_WithValidReservation_ShouldReturnTrue()
        {
            var reservationId = 1;
            var userId = 1;

            var reservation = new Reservation
            {
                Id = reservationId,
                UserId = userId,
                Status = "Confirmed",
                FlightId = 1
            };

            _reservationRepositoryMock.Setup(r => r.GetByCriteriaAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
                .ReturnsAsync(new List<Reservation> { reservation });

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _reservationService.CancelReservationAsync(reservationId, userId);

            // Assert
            result.Should().BeTrue();
            reservation.Status.Should().Be("Cancelled");
            reservation.CancelledAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CancelReservationAsync_WithNonExistentReservation_ShouldReturnFalse()
        {
            var reservationId = 999;
            var userId = 1;

            _reservationRepositoryMock.Setup(r => r.GetByCriteriaAsync(It.IsAny<Expression<Func<Reservation, bool>>>()))
                .ReturnsAsync(new List<Reservation>());

            // Act
            var result = await _reservationService.CancelReservationAsync(reservationId, userId);

            // Assert
            result.Should().BeFalse();
        }
    }
}

