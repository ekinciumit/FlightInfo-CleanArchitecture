using FlightInfo.Shared.DTOs;
using FlightInfo.Application.Contracts.Auth;

namespace FlightInfo.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserAsync(int id);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<UserDto> UpdateProfileAsync(int userId, string? fullName, string? phone);
        Task<bool> DeleteUserAsync(int id);
    }
}

