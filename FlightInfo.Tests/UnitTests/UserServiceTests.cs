using FluentAssertions;
using FlightInfo.Application.Services;
using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using FlightInfo.Shared.DTOs;
using FlightInfo.Application.Contracts.Auth;
using Moq;
using Xunit;

namespace FlightInfo.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        #region GetUsersAsync Tests

        [Fact]
        public async Task GetUsersAsync_ShouldReturnAllNonDeletedUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Email = "user1@example.com", FullName = "User One", Role = "User", IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Email = "user2@example.com", FullName = "User Two", Role = "Admin", IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new User { Id = 3, Email = "user3@example.com", FullName = "User Three", Role = "User", IsDeleted = true, CreatedAt = DateTime.UtcNow }
            };

            _userRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(u => !u.Email.Contains("user3"));
        }

        [Fact]
        public async Task GetUsersAsync_WithNoUsers_ShouldReturnEmptyList()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region GetUserAsync Tests

        [Fact]
        public async Task GetUserAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                Role = "User",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Email.Should().Be("user@example.com");
            result.FullName.Should().Be("Test User");
            result.Role.Should().Be("User");
        }

        [Fact]
        public async Task GetUserAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var userId = 999;

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserAsync_WithDeletedUser_ShouldReturnNull()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                Role = "User",
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region UpdateUserAsync Tests

        [Fact]
        public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "old@example.com",
                FullName = "Old Name",
                Role = "User",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            var updateRequest = new UpdateUserRequest
            {
                FullName = "New Name",
                Email = "new@example.com"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateRequest);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be("New Name");
            result.Email.Should().Be("old@example.com"); // Email is not updated in the service
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var userId = 999;
            var updateRequest = new UpdateUserRequest
            {
                FullName = "New Name",
                Email = "new@example.com"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserAsync(userId, updateRequest));
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_WithValidUser_ShouldSoftDeleteUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                Role = "User",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Should().BeTrue();
            user.IsDeleted.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistentUser_ShouldReturnFalse()
        {
            // Arrange
            var userId = 999;

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteUserAsync_WithAlreadyDeletedUser_ShouldStillReturnTrue()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "user@example.com",
                FullName = "Test User",
                Role = "User",
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.Should().BeTrue(); // Service doesn't check if already deleted
        }

        #endregion
    }
}

