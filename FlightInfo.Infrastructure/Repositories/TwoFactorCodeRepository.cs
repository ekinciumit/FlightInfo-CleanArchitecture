using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightInfo.Infrastructure.Repositories
{
    public class TwoFactorCodeRepository : Repository<TwoFactorCode>, ITwoFactorCodeRepository
    {
        public TwoFactorCodeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<TwoFactorCode?> GetValidCodeAsync(int userId, string code, string type)
        {
            return await _context.TwoFactorCodes
                .Where(x =>
                    x.UserId == userId &&
                    x.Code == code &&
                    x.Type == type &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt) // En son oluşturulan kodu al
                .FirstOrDefaultAsync();
        }

        public async Task<List<TwoFactorCode>> GetExpiredCodesAsync(int userId)
        {
            return await _context.TwoFactorCodes
                .Where(x => x.UserId == userId && x.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<TwoFactorCode>> GetUserCodesAsync(int userId)
        {
            return await _context.TwoFactorCodes
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}

