using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using FlightInfo.Application.Contracts.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlightInfo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = u.Role.ToString(),
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    Phone = u.Phone,
                    IsActive = u.IsActive
                });
        }

        public async Task<UserDto?> GetUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Phone = user.Phone,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                throw new ArgumentException("User not found");

            // Email güncellenemez - güvenlik nedeniyle
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Role))
                user.Role = request.Role;

            // Phone güncellemesi
            if (request.Phone != null)
                user.Phone = request.Phone;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Phone = user.Phone,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, string? fullName, string? phone)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!string.IsNullOrWhiteSpace(fullName))
                user.FullName = fullName;

            // phone alanı domain'de opsiyonel mevcut
            if (phone != null)
                user.Phone = phone;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Phone = user.Phone,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Soft delete
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return false;

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}


