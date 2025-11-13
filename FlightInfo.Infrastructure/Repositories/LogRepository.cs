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
    /// Log repository implementation using Entity Framework Core
    /// </summary>
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _context;

        public LogRepository(AppDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async Task<Log?> GetByIdAsync(int id)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Log>> GetAllAsync()
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .ToListAsync();
        }

        public async Task<Log> AddAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            return log;
        }

        public async Task UpdateAsync(Log log)
        {
            _context.Logs.Update(log);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Log log)
        {
            _context.Logs.Remove(log);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Log>> FindAsync(Expression<Func<Log, bool>> predicate)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Log?> FirstOrDefaultAsync(Expression<Func<Log, bool>> predicate)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Log>> GetByUserIdAsync(int userId)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByFlightIdAsync(int flightId)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .Where(l => l.FlightId == flightId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByLevelAsync(string level)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .Where(l => l.Level == level)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Logs
                .Include(l => l.User)
                .Include(l => l.Flight)
                .Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<int> ClearAllAsync()
        {
            var logs = await _context.Logs.ToListAsync();
            int count = logs.Count;
            _context.Logs.RemoveRange(logs);
            return count;
        }
    }
}

