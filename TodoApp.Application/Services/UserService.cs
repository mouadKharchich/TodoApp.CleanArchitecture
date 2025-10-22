using System.IdentityModel.Tokens.Jwt;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Dtos.User;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Application.Mappings;

namespace TodoApp.Application.Services;

/// <summary>
/// Service layer for handling business logic related to users.
/// Interacts with the User repository and performs mapping between entities and DTOs.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IDatabaseTransaction _transaction;
    
    public UserService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IDatabaseTransaction transaction)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _transaction = transaction;
    }

    #region Public Methods
    
    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A collection of <see cref="UserResponseDto"/> representing all users.</returns>
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(UserMapper.ToDto);
    }

    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="uid">The unique ID of the user.</param>
    /// <returns>A <see cref="UserResponseDto"/> if found; otherwise, throws <see cref="NotFoundException"/>.</returns>
    public async Task<UserResponseDto?> GetUserByIdAsync(Guid? uid)
    {
        if (!uid.HasValue || uid == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be null or empty.");
        }
        
        var userEntity = await _userRepository.GetUserByIdAsync(uid);
        if (userEntity is null)
        {
            throw new NotFoundException($"User with ID {uid} not found");
        }

        return UserMapper.ToDto(userEntity);
    }

    /// <summary>
    /// Retrieves a user by ID including their related tasks.
    /// Useful when you want user details along with task associations.
    /// </summary>
    /// <param name="uid">The unique ID of the user.</param>
    /// <returns>A <see cref="UserResponseDto"/> with associated tasks if found; otherwise, throws <see cref="NotFoundException"/>.</returns>
    public async Task<UserResponseDto?> GetUserByIdWithTasksAsync(Guid? uid)
    {
        if (!uid.HasValue || uid == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be null or empty.");
        }
        var userEntity = await _userRepository.GetUserByIdAsync(uid);
        if (userEntity is null)
        {
            throw new NotFoundException($"User with ID {uid} not found");
        }

        return UserMapper.ToDtoWithTasks(userEntity);
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerUserDto">The DTO containing user data to create.</param>
    /// <returns>The created <see cref="UserResponseDto"/>.</returns>
    public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        //check if email is already exist
        var existingUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);
        if (existingUser != null)
        {
            throw new ArgumentException($"Email {registerUserDto.Email} already exists");
        }
        
        // Map the DTO to an entity
        var userEntity = UserMapper.ToEntity(registerUserDto);

        // Hash the user's password before saving
        userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
        
        // Persist the entity using repository
        await _userRepository.CreateUserAsync(userEntity);
        await _transaction.SaveChangesAsync();
        
        // Get CreatedUser 
        var createdUser = await _userRepository.GetUserByEmailAsync(registerUserDto.Email);

        // Map back to response DTO
        return UserMapper.ToDto(createdUser);
    }

    
    /// <summary>
    /// Logs in a user by verifying their credentials.
    /// </summary>
    /// <param name="dto">The login DTO containing email and password.</param>
    /// <returns>The <see cref="LoginUserResponseDto"/> if credentials are valid.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if validation or authentication fails.</exception>
    public async Task<LoginUserResponseDto> LoginAsync(LoginUserDto dto)
    {
        ValidateLoginDto(dto);
        var user = await AuthenticateUserAsync(dto);

        var token = _jwtService.GenerateJwtToken(user);

        return new LoginUserResponseDto
        {
            AcessToken = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}",
            Username = user.Username,
            ExpiresIn = (int)(token.ValidTo - DateTime.UtcNow).TotalSeconds
        };
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Validates the login DTO.
    /// </summary>
    /// <param name="dto">Login DTO to validate.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown if email or password is missing.</exception>
    private void ValidateLoginDto(LoginUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
        {
            throw new UnauthorizedAccessException("Incorrect Email or Password.");
        }
    }

    /// <summary>
    /// Authenticates the user by email and password.
    /// </summary>
    /// <param name="dto">Login DTO.</param>
    /// <returns>The authenticated user entity.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if user not found or password invalid.</exception>
    private async Task<UserResponseDto> AuthenticateUserAsync(LoginUserDto dto)
    {
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Incorrect Email or Password.");
        }

        return UserMapper.ToDto(user);
    }

    #endregion

}
