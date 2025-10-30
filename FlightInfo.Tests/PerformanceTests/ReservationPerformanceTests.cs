using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FlightInfo.Infrastructure.Data;
using System.Diagnostics;
using Xunit.Abstractions;
using System.Net.Http.Json;
using System.Net;

namespace FlightInfo.Tests.PerformanceTests
{
    public class ReservationPerformanceTests
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public ReservationPerformanceTests(ITestOutputHelper output)
        {
            // Test için basit HTTP client
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:7104");
            _output = output;
        }

        [Fact]
        public async Task GetUserReservations_ShouldCompleteWithinAcceptableTime()
        {
            // Arrange
            var maxResponseTime = TimeSpan.FromSeconds(2); // 2 saniye maksimum
            var stopwatch = Stopwatch.StartNew();

            // Act
            var response = await _client.GetAsync($"/api/reservation");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
            stopwatch.Elapsed.Should().BeLessThan(maxResponseTime);

            _output.WriteLine($"GetUserReservations completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        [Fact]
        public async Task CreateReservation_ShouldCompleteWithinAcceptableTime()
        {
            // Arrange
            var reservationRequest = new
            {
                FlightId = 1,
                FlightPriceId = 1,
                SeatNumber = "P1"
            };

            var maxResponseTime = TimeSpan.FromSeconds(3); // 3 saniye maksimum
            var stopwatch = Stopwatch.StartNew();

            // Act
            var response = await _client.PostAsJsonAsync("/api/reservation", reservationRequest);
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); // Authentication gerekli
            stopwatch.Elapsed.Should().BeLessThan(maxResponseTime);

            _output.WriteLine($"CreateReservation completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        [Fact]
        public async Task MultipleConcurrentRequests_ShouldHandleGracefully()
        {
            // Arrange
            var concurrentRequests = 10;
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < concurrentRequests; i++)
            {
                tasks.Add(_client.GetAsync($"/api/reservation"));
            }

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            responses.Should().HaveCount(concurrentRequests);
            responses.All(r => r.StatusCode == HttpStatusCode.Unauthorized).Should().BeTrue(); // Authentication gerekli
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));

            _output.WriteLine($"Handled {concurrentRequests} concurrent requests in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}

