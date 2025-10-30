using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Interfaces.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightDto>> GetFlightsAsync();
        Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchCriteria criteria);
        Task<IEnumerable<object>> GetFlightsWithPricesAsync();
        Task<IEnumerable<object>> GetFlightPricesAsync(int flightId);
        Task<object> GetFlightStatusAsync(int flightId);
        Task<FlightDto?> GetFlightAsync(int id);
        Task<FlightDto> CreateFlightAsync(FlightDto flightDto);
        Task<FlightDto> UpdateFlightAsync(int id, FlightDto flightDto);
        Task<bool> DeleteFlightAsync(int id);
        Task<FlightDto> RestoreFlightAsync(int id);
        Task<IEnumerable<FlightDto>> GetDeletedFlightsAsync();
        Task<bool> AddFlightPriceAsync(int flightId, FlightPriceDto priceDto);
        Task<bool> UpdateFlightPriceAsync(int flightId, int priceId, FlightPriceDto priceDto);
        Task<bool> DeleteFlightPriceAsync(int flightId, int priceId);
    }
}

