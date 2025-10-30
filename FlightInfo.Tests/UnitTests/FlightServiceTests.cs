using FluentAssertions;
using FlightInfo.Application.Services;
using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using AutoMapper;
using Moq;
using Xunit;
using System.Linq.Expressions;

namespace FlightInfo.Tests.UnitTests
{
    public class FlightServiceTests
    {
        private readonly Mock<IFlightRepository> _flightRepositoryMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly FlightService _flightService;

        public FlightServiceTests()
        {
            _flightRepositoryMock = new Mock<IFlightRepository>();
            _logServiceMock = new Mock<ILogService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _flightService = new FlightService(
                _flightRepositoryMock.Object,
                _logServiceMock.Object,
                _cacheServiceMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object);
        }

        #region GetFlightsAsync Tests

        [Fact]
        public async Task GetFlightsAsync_WithCachedData_ShouldReturnCachedFlights()
        {
            // Arrange
            var cachedFlights = new List<FlightDto>
            {
                new FlightDto { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR" },
                new FlightDto { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG" }
            };

            _cacheServiceMock.Setup(c => c.GetAsync<IEnumerable<FlightDto>>("flights_all"))
                .ReturnsAsync(cachedFlights);

            // Act
            var result = await _flightService.GetFlightsAsync();

            // Assert
            result.Should().BeEquivalentTo(cachedFlights);
            _flightRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
            _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task GetFlightsAsync_WithoutCachedData_ShouldReturnFlightsFromDatabase()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR", IsDeleted = false },
                new Flight { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG", IsDeleted = false },
                new Flight { Id = 3, FlightNumber = "TK003", Origin = "IST", Destination = "FRA", IsDeleted = true }
            };

            var flightDtos = new List<FlightDto>
            {
                new FlightDto { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR" },
                new FlightDto { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG" }
            };

            _cacheServiceMock.Setup(c => c.GetAsync<IEnumerable<FlightDto>>("flights_all"))
                .ReturnsAsync((IEnumerable<FlightDto>)null);

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            _mapperMock.Setup(m => m.Map<IEnumerable<FlightDto>>(It.IsAny<IEnumerable<Flight>>()))
                .Returns(flightDtos);

            // Act
            var result = await _flightService.GetFlightsAsync();

            // Assert
            result.Should().BeEquivalentTo(flightDtos);
            _flightRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            _cacheServiceMock.Verify(c => c.SetAsync("flights_all", It.IsAny<IEnumerable<FlightDto>>(), TimeSpan.FromMinutes(5)), Times.Once);
        }

        #endregion

        #region SearchFlightsAsync Tests

        [Fact]
        public async Task SearchFlightsAsync_WithValidCriteria_ShouldReturnFilteredFlights()
        {
            // Arrange
            var criteria = new FlightSearchCriteria
            {
                Origin = "IST",
                Destination = "LHR",
                DepartureDate = DateTime.Today,
                UserId = 1
            };

            var flights = new List<Flight>
            {
                new Flight
                {
                    Id = 1,
                    FlightNumber = "TK001",
                    Origin = "IST",
                    Destination = "LHR",
                    DepartureTime = DateTime.Today.AddHours(10),
                    Status = "Active",
                    IsDeleted = false
                },
                new Flight
                {
                    Id = 2,
                    FlightNumber = "TK002",
                    Origin = "IST",
                    Destination = "CDG",
                    DepartureTime = DateTime.Today.AddHours(14),
                    Status = "Active",
                    IsDeleted = false
                }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.SearchFlightsAsync(criteria);

            // Assert
            result.Should().HaveCount(1);
            result.First().Origin.Should().Be("IST");
            result.First().Destination.Should().Be("LHR");
            _logServiceMock.Verify(l => l.LogFlightSearchAsync(1, "IST -> LHR", "IST", "LHR", DateTime.Today), Times.Once);
        }

        [Fact]
        public async Task SearchFlightsAsync_WithEmptyCriteria_ShouldReturnAllFlights()
        {
            // Arrange
            var criteria = new FlightSearchCriteria();

            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR", IsDeleted = false },
                new Flight { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG", IsDeleted = false }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.SearchFlightsAsync(criteria);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task SearchFlightsAsync_WithSpecificStatus_ShouldReturnFlightsWithThatStatus()
        {
            // Arrange
            var criteria = new FlightSearchCriteria
            {
                Status = "Active"
            };

            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR", Status = "Active", IsDeleted = false },
                new Flight { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG", Status = "Cancelled", IsDeleted = false }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.SearchFlightsAsync(criteria);

            // Assert
            result.Should().HaveCount(1);
            result.First().Status.Should().Be("Active");
        }

        [Fact]
        public async Task SearchFlightsAsync_WithDepartureDate_ShouldReturnFlightsOnThatDate()
        {
            // Arrange
            var specificDate = DateTime.Today.AddDays(1);
            var criteria = new FlightSearchCriteria
            {
                DepartureDate = specificDate
            };

            var flights = new List<Flight>
            {
                new Flight
                {
                    Id = 1,
                    FlightNumber = "TK001",
                    Origin = "IST",
                    Destination = "LHR",
                    DepartureTime = specificDate.AddHours(10),
                    IsDeleted = false
                },
                new Flight
                {
                    Id = 2,
                    FlightNumber = "TK002",
                    Origin = "IST",
                    Destination = "CDG",
                    DepartureTime = DateTime.Today.AddHours(14),
                    IsDeleted = false
                }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.SearchFlightsAsync(criteria);

            // Assert
            result.Should().HaveCount(1);
            result.First().DepartureTime.Date.Should().Be(specificDate.Date);
        }

        [Fact]
        public async Task SearchFlightsAsync_ShouldExcludeDeletedFlights()
        {
            // Arrange
            var criteria = new FlightSearchCriteria();

            var flights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "TK001", Origin = "IST", Destination = "LHR", IsDeleted = false },
                new Flight { Id = 2, FlightNumber = "TK002", Origin = "IST", Destination = "CDG", IsDeleted = true }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.SearchFlightsAsync(criteria);

            // Assert
            result.Should().HaveCount(1);
            result.First().FlightNumber.Should().Be("TK001");
        }

        #endregion

        #region GetFlightsWithPricesAsync Tests

        [Fact]
        public async Task GetFlightsWithPricesAsync_ShouldReturnFlightsWithPrices()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight
                {
                    Id = 1,
                    FlightNumber = "TK001",
                    Origin = "IST",
                    Destination = "LHR",
                    FlightPrices = new List<FlightPrice>
                    {
                        new FlightPrice { Id = 1, Class = "Economy", Price = 100.0m, Currency = "USD" },
                        new FlightPrice { Id = 2, Class = "Business", Price = 300.0m, Currency = "USD" }
                    },
                    IsDeleted = false
                }
            };

            _flightRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(flights);

            // Act
            var result = await _flightService.GetFlightsWithPricesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        #endregion
    }
}

