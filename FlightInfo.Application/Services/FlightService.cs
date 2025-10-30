using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using AutoMapper;

namespace FlightInfo.Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ILogService _log;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FlightService(IFlightRepository flightRepository, ILogService log, ICacheService cacheService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _flightRepository = flightRepository;
            _log = log;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlightDto>> GetFlightsAsync()
        {
            const string cacheKey = "flights_all";

            var cachedFlights = await _cacheService.GetAsync<IEnumerable<FlightDto>>(cacheKey);
            if (cachedFlights != null)
            {
                return cachedFlights;
            }

            var flights = await _flightRepository.GetAllAsync();

            // Silinmemiş tüm uçuşlar
            var activeFlights = flights.Where(f => !f.IsDeleted);

            var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(activeFlights);

            await _cacheService.SetAsync(cacheKey, flightDtos, TimeSpan.FromMinutes(5));

            return flightDtos;
        }

        public async Task<IEnumerable<FlightDto>> SearchFlightsAsync(FlightSearchCriteria criteria)
        {
            // Log flight search (null-safe)
            var originSafe = criteria.Origin ?? string.Empty;
            var destinationSafe = criteria.Destination ?? string.Empty;
            await _log.LogFlightSearchAsync(
                criteria.UserId,
                $"{originSafe} -> {destinationSafe}",
                originSafe,
                destinationSafe,
                criteria.DepartureDate
            );

            // Repository'den tüm uçuşları al
            var allFlights = await _flightRepository.GetAllAsync();
            var query = allFlights.AsQueryable();

            // Filtreleme
            if (!string.IsNullOrEmpty(criteria.Origin))
                query = query.Where(f => f.Origin.Contains(criteria.Origin));

            if (!string.IsNullOrEmpty(criteria.Destination))
                query = query.Where(f => f.Destination.Contains(criteria.Destination));

            if (criteria.DepartureDate.HasValue)
            {
                var departureDate = criteria.DepartureDate.Value.Date;
                query = query.Where(f => f.DepartureTime.Date == departureDate);
            }

            if (!string.IsNullOrEmpty(criteria.Status))
                query = query.Where(f => f.Status == criteria.Status);

            // Silinmemiş uçuşları getir
            query = query.Where(f => !f.IsDeleted);

            var flights = query
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    Status = f.Status,
                    AircraftType = f.AircraftType,
                    Capacity = f.Capacity
                });

            return flights;
        }

        public async Task<IEnumerable<object>> GetFlightsWithPricesAsync()
        {
            var flights = await _flightRepository.GetAllAsync();
            var flightsWithPrices = flights
                .Where(f => !f.IsDeleted)
                .Select(f => new
                {
                    f.Id,
                    f.FlightNumber,
                    f.Origin,
                    f.Destination,
                    f.DepartureTime,
                    f.ArrivalTime,
                    f.Status,
                    f.AircraftType,
                    f.Capacity,
                    f.AvailableSeats,
                    Prices = f.FlightPrices.Select(fp => new
                    {
                        fp.Id,
                        fp.Class,
                        fp.Price,
                        fp.Currency,
                        fp.AvailableSeats
                    })
                });

            return flightsWithPrices;
        }

        public async Task<IEnumerable<object>> GetFlightPricesAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null) return new List<object>();

            var prices = flight.FlightPrices.Select(fp => new
            {
                fp.Id,
                fp.Class,
                fp.Price,
                fp.Currency,
                fp.AvailableSeats
            });

            return prices;
        }

        public async Task<object> GetFlightStatusAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null) throw new ArgumentException("Flight not found");

            return new
            {
                flight.Id,
                flight.FlightNumber,
                flight.Status,
                flight.DepartureTime,
                flight.ArrivalTime
            };
        }

        public async Task<FlightDto?> GetFlightAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null || flight.IsDeleted) return null;

            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftType = flight.AircraftType,
                Capacity = flight.Capacity
            };
        }

        public async Task<FlightDto> CreateFlightAsync(FlightDto flightDto)
        {
            var flight = new Flight
            {
                FlightNumber = flightDto.FlightNumber,
                Origin = flightDto.Origin,
                Destination = flightDto.Destination,
                DepartureTime = flightDto.DepartureTime,
                ArrivalTime = flightDto.ArrivalTime,
                Status = flightDto.Status,
                AircraftType = flightDto.AircraftType,
                Capacity = flightDto.Capacity
            };

            await _flightRepository.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("flights_all");

            // Log
            await _log.LogAsync("Flight Created", null, flight.Id);

            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftType = flight.AircraftType,
                Capacity = flight.Capacity
            };
        }

        public async Task<FlightDto> UpdateFlightAsync(int id, FlightDto flightDto)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new ArgumentException("Flight not found");

            flight.FlightNumber = flightDto.FlightNumber;
            flight.Origin = flightDto.Origin;
            flight.Destination = flightDto.Destination;
            flight.DepartureTime = flightDto.DepartureTime;
            flight.ArrivalTime = flightDto.ArrivalTime;
            flight.Status = flightDto.Status;
            flight.AircraftType = flightDto.AircraftType;
            flight.Capacity = flightDto.Capacity;

            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("flights_all");

            // Log
            await _log.LogAsync("Flight Updated", null, flight.Id);

            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftType = flight.AircraftType,
                Capacity = flight.Capacity
            };
        }

        public async Task<bool> DeleteFlightAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                return false;

            // Soft delete
            flight.IsDeleted = true;
            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("flights_all");

            // Log
            await _log.LogAsync("Flight Deleted", null, flight.Id);

            return true;
        }

        public async Task<FlightDto> RestoreFlightAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new ArgumentException("Flight not found");

            flight.Status = "Scheduled";
            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("flights_all");

            // Log
            await _log.LogAsync("Flight Restored", null, flight.Id);

            return new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Status = flight.Status,
                AircraftType = flight.AircraftType,
                Capacity = flight.Capacity
            };
        }

        public async Task<IEnumerable<FlightDto>> GetDeletedFlightsAsync()
        {
            var flights = await _flightRepository.GetByCriteriaAsync(f => f.IsDeleted);

            return flights.Select(f => new FlightDto
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                Origin = f.Origin,
                Destination = f.Destination,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                Status = f.Status,
                AircraftType = f.AircraftType,
                Capacity = f.Capacity
            });
        }

        public async Task<bool> AddFlightPriceAsync(int flightId, FlightPriceDto priceDto)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                return false;

            var flightPrice = new FlightPrice
            {
                FlightId = flightId,
                Class = priceDto.Class,
                Price = priceDto.Price,
                Currency = priceDto.Currency,
                AvailableSeats = priceDto.AvailableSeats,
                CreatedAt = DateTime.UtcNow
            };

            flight.FlightPrices.Add(flightPrice);
            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Log
            await _log.LogAsync("Flight Price Added", null, flightId);

            return true;
        }

        public async Task<bool> UpdateFlightPriceAsync(int flightId, int priceId, FlightPriceDto priceDto)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                return false;

            var flightPrice = flight.FlightPrices.FirstOrDefault(fp => fp.Id == priceId);
            if (flightPrice == null)
            {
                // Debug için log ekle
                Console.WriteLine($"Price not found: FlightId={flightId}, PriceId={priceId}");
                Console.WriteLine($"Available prices: {string.Join(", ", flight.FlightPrices.Select(fp => fp.Id))}");
                return false;
            }

            flightPrice.Class = priceDto.Class;
            flightPrice.Price = priceDto.Price;
            flightPrice.Currency = priceDto.Currency;
            flightPrice.AvailableSeats = priceDto.AvailableSeats;

            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Log
            await _log.LogAsync("Flight Price Updated", null, flightId);

            return true;
        }

        public async Task<bool> DeleteFlightPriceAsync(int flightId, int priceId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                return false;

            var flightPrice = flight.FlightPrices.FirstOrDefault(fp => fp.Id == priceId);
            if (flightPrice == null)
                return false;

            flight.FlightPrices.Remove(flightPrice);
            await _flightRepository.UpdateAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Log
            await _log.LogAsync("Flight Price Deleted", null, flightId);

            return true;
        }
    }
}


