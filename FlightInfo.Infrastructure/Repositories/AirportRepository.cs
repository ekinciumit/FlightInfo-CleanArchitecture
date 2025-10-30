using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightInfo.Infrastructure.Repositories
{
    /// <summary>
    /// Airport repository implementation using Entity Framework Core
    /// </summary>
    public class AirportRepository : IAirportRepository
    {
        private readonly AppDbContext _context;

        public AirportRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Airport?> GetByIdAsync(int id)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Airport>> GetAllAsync()
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .ToListAsync();
        }

        public async Task<IEnumerable<Airport>> GetByCriteriaAsync(Expression<Func<Airport, bool>> predicate)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Airport>> GetByCityIdAsync(int cityId)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .Where(a => a.CityId == cityId)
                .ToListAsync();
        }

        public async Task<Airport?> GetByCodeAsync(string code)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task<IEnumerable<Airport>> GetByCountryCodeAsync(string countryCode)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .Where(a => a.City.Country.Code == countryCode)
                .ToListAsync();
        }

        public async Task<Airport> AddAsync(Airport airport)
        {
            await _context.Airports.AddAsync(airport);
            return airport;
        }

        public async Task UpdateAsync(Airport airport)
        {
            _context.Airports.Update(airport);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Airport airport)
        {
            _context.Airports.Remove(airport);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Airport>> FindAsync(Expression<Func<Airport, bool>> predicate)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Airport?> FirstOrDefaultAsync(Expression<Func<Airport, bool>> predicate)
        {
            return await _context.Airports
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Airports.AnyAsync(a => a.Id == id);
        }
    }
}

