using FlightInfo.Domain.Entities;

namespace FlightInfo.Application.Interfaces.Repositories
{
    public interface ITwoFactorCodeRepository : IRepository<TwoFactorCode>
    {
        Task<TwoFactorCode?> GetValidCodeAsync(int userId, string code, string type);
        Task<List<TwoFactorCode>> GetExpiredCodesAsync(int userId);
        Task<List<TwoFactorCode>> GetUserCodesAsync(int userId);
    }
}



