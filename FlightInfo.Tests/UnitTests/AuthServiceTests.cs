using FluentAssertions;
using FlightInfo.Application.Services;
using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using System.Linq.Expressions;

namespace FlightInfo.Tests.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _configMock = new Mock<IConfiguration>();
            _logServiceMock = new Mock<ILogService>();

            // JWT configuration setup
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("SuperSecretKeyThatIsAtLeast32CharactersLong!");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("FlightInfo");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("FlightInfoUsers");
            jwtSectionMock.Setup(x => x["ExpireMinutes"]).Returns("60");

            _configMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _configMock.Object,
                _logServiceMock.Object);
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
        {
            var email = "test@example.com";
            var fullName = "John Doe";
            var password = "password123";

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(email))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("User registered successfully.");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyEmail_ShouldReturnFailure()
        {
            var email = "";
            var fullName = "John Doe";
            var password = "password123";

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email adresi gerekli.");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyFullName_ShouldReturnFailure()
        {
            var email = "test@example.com";
            var fullName = "";
            var password = "password123";

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Ad soyad gerekli.");
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyPassword_ShouldReturnFailure()
        {
            var email = "test@example.com";
            var fullName = "John Doe";
            var password = "";

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Şifre gerekli.");
        }

        [Fact]
        public async Task RegisterAsync_WithShortPassword_ShouldReturnFailure()
        {
            var email = "test@example.com";
            var fullName = "John Doe";
            var password = "12345";

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Şifre en az 6 karakter olmalı.");
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidEmail_ShouldReturnFailure()
        {
            var email = "invalid-email";
            var fullName = "John Doe";
            var password = "password123";

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Geçerli bir email adresi giriniz.");
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
        {
            var email = "existing@example.com";
            var fullName = "John Doe";
            var password = "password123";

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(email, fullName, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Bu email adresi zaten kayıtlı.");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
        {
            var email = "test@example.com";
            var password = "password123";
            var user = new User
            {
                Id = 1,
                Email = email,
                FullName = "John Doe",
                Role = "User",
                PasswordHash = new byte[64], // Will be set by CreatePasswordHash
                PasswordSalt = new byte[128] // Will be set by CreatePasswordHash
            };

            // Create actual password hash for testing
            _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Giriş başarılı.");
            result.Token.Should().NotBeNullOrEmpty();
            result.User.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_WithEmptyEmail_ShouldReturnFailure()
        {
            var email = "";
            var password = "password123";

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email adresi gerekli.");
            result.Token.Should().BeEmpty();
            result.User.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithEmptyPassword_ShouldReturnFailure()
        {
            var email = "test@example.com";
            var password = "";

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Şifre gerekli.");
            result.Token.Should().BeEmpty();
            result.User.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldReturnFailure()
        {
            var email = "nonexistent@example.com";
            var password = "password123";

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email veya şifre hatalı.");
            result.Token.Should().BeEmpty();
            result.User.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
        {
            var email = "test@example.com";
            var correctPassword = "password123";
            var wrongPassword = "wrongpassword";
            var user = new User
            {
                Id = 1,
                Email = email,
                FullName = "John Doe",
                Role = "User",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[128]
            };

            // Create password hash with correct password
            _authService.CreatePasswordHash(correctPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, wrongPassword);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email veya şifre hatalı.");
            result.Token.Should().BeEmpty();
            result.User.Should().BeNull();
        }

        #endregion

        #region Password Hash Tests

        [Fact]
        public void CreatePasswordHash_ShouldCreateValidHash()
        {
            var password = "testpassword123";

            // Act
            _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            // Assert
            passwordHash.Should().NotBeNull();
            passwordSalt.Should().NotBeNull();
            passwordHash.Length.Should().Be(64); // SHA512 hash length
            passwordSalt.Length.Should().Be(128); // SHA512 key length
        }

        [Fact]
        public void VerifyPasswordHash_WithCorrectPassword_ShouldReturnTrue()
        {
            var password = "testpassword123";
            _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            // Act
            var result = _authService.VerifyPasswordHash(password, passwordHash, passwordSalt);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPasswordHash_WithIncorrectPassword_ShouldReturnFalse()
        {
            var correctPassword = "testpassword123";
            var wrongPassword = "wrongpassword";
            _authService.CreatePasswordHash(correctPassword, out byte[] passwordHash, out byte[] passwordSalt);

            // Act
            var result = _authService.VerifyPasswordHash(wrongPassword, passwordHash, passwordSalt);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region JWT Token Tests

        [Fact]
        public void GenerateJwtToken_WithValidUser_ShouldReturnValidToken()
        {
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FullName = "John Doe",
                Role = "User"
            };

            // Act
            var token = _authService.GenerateJwtToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().Contain("."); // JWT tokens have dots
        }

        #endregion
    }
}

