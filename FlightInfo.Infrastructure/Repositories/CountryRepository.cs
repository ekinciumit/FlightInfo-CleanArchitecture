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
    /// Country repository implementation using Entity Framework Core
    /// </summary>
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .ToListAsync();
        }

        public async Task<IEnumerable<Country>> GetByCriteriaAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Country?> GetByCodeAsync(string code)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Country>> GetAllWithCitiesAsync()
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .ToListAsync();
        }

        public async Task<Country> AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            return country;
        }

        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Country country)
        {
            _context.Countries.Remove(country);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Country>> FindAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Country?> FirstOrDefaultAsync(Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Countries.AnyAsync(c => c.Id == id);
        }
    }
}

