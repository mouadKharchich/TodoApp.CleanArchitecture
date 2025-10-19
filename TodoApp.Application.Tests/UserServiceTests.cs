using System.IdentityModel.Tokens.Jwt;
using Moq;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Application.Services;
using TodoApp.Domain.Entities;
using Xunit;

namespace TodoApp.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _userService = new UserService(_mockUserRepository.Object, _mockJwtService.Object);
    }
    
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var mockUsers = new List<User>
        {
            new User { PublicId = Guid.NewGuid(), Username = "Alice", Email = "alice@test.com" },
            new User { PublicId = Guid.NewGuid(), Username = "Bob", Email = "bob@test.com" }
        };

        _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(mockUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Username == "Alice");
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUser = new User { PublicId = userId, Username = "John", Email = "john@test.com" };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(mockUser);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByIdAsync(userId));
    }

    [Fact]
    public async Task GetUserByIdWithTasksAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUser = new User { PublicId = userId, Username = "Jane", Email = "jane@test.com" };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(mockUser);

        // Act
        var result = await _userService.GetUserByIdWithTasksAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.Username);
    }
    
    [Fact]
    public async Task GetUserByIdWithTasksAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByIdWithTasksAsync(userId));
    }
    
    [Fact]
    public async Task RegisterUserAsync_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Username = "TestUser",
            Email = "unique@test.com",
            Password = "Password123"
        };

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);

        var createdUser = new User { PublicId = Guid.NewGuid(), Username = "TestUser", Email = "unique@test.com" };
        _mockUserRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.RegisterUserAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestUser", result.Username);
    }
    
    [Fact]
    public async Task RegisterUserAsync_ShouldThrowArgumentException_WhenEmailExists()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Username = "DuplicateUser",
            Email = "existing@test.com",
            Password = "Password123"
        };

        var existingUser = new User { PublicId = Guid.NewGuid(), Email = "existing@test.com" };
        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.RegisterUserAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginUserDto
        {
            Email = "user@test.com",
            Password = "Password123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User { PublicId = Guid.NewGuid(), Email = "user@test.com", Username = "TestUser", PasswordHash = hashedPassword };

        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);

        var mockToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(30));
        _mockJwtService.Setup(j => j.GenerateJwtToken(It.IsAny<UserResponseDto>())).Returns(mockToken);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("Bearer ", result.AcessToken);
        Assert.Equal("TestUser", result.Username);
        Assert.True(result.ExpiresIn > 0);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "wrong@test.com", Password = "bad" };
        _mockUserRepository.Setup(r => r.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenMissingEmailOrPassword()
    {
        // Arrange
        var invalidDto = new LoginUserDto { Email = "", Password = "" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(invalidDto));
    }
    
}