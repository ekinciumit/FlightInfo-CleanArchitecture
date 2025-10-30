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
    /// City repository implementation using Entity Framework Core
    /// </summary>
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;

        public CityRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .ToListAsync();
        }

        public async Task<IEnumerable<City>> GetByCriteriaAsync(Expression<Func<City, bool>> predicate)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<City>> GetByCountryIdAsync(int countryId)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .Where(c => c.CountryId == countryId)
                .ToListAsync();
        }

        public async Task<City?> GetByCodeAsync(string code)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<City>> GetAllWithAirportsAsync()
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .ToListAsync();
        }

        public async Task<City> AddAsync(City city)
        {
            _context.Cities.Add(city);
            return city;
        }

        public async Task UpdateAsync(City city)
        {
            _context.Cities.Update(city);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(City city)
        {
            _context.Cities.Remove(city);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<City>> FindAsync(Expression<Func<City, bool>> predicate)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<City?> FirstOrDefaultAsync(Expression<Func<City, bool>> predicate)
        {
            return await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.Airports)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Cities.AnyAsync(c => c.Id == id);
        }
    }
}

