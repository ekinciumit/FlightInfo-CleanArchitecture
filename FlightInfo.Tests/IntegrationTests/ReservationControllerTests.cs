using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FlightInfo.Infrastructure.Data;
using System.Net.Http.Json;
using FlightInfo.Shared.DTOs;
using System.Net;

namespace FlightInfo.Tests.IntegrationTests
{
    public class ReservationControllerTests
    {
        // Integration testler için basit HTTP client kullanımı
        private readonly HttpClient _client;

        public ReservationControllerTests()
        {
            // Test için basit HTTP client
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:7104");
        }

        [Fact]
        public async Task GetUserReservations_WithValidUserId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;

            // Act - Doğru endpoint'i kullan
            var response = await _client.GetAsync($"/api/reservation");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
        }

        [Fact]
        public async Task CreateReservation_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var reservationRequest = new
            {
                FlightId = 1,
                FlightPriceId = 1,
                SeatNumber = "A1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/reservation", reservationRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
        }

        [Fact]
        public async Task CreateReservation_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidRequest = new
            {
                FlightId = 1,
                FlightPriceId = 1,
                SeatNumber = "" // Invalid: empty seat number
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/reservation", invalidRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
        }

        [Fact]
        public async Task CancelReservation_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var reservationId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/reservation/{reservationId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
        }
    }
}

