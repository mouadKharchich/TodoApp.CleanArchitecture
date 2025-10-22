using TodoApp.Application.Dtos.User;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.IServices;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(Guid? uid);
    Task<UserResponseDto?> GetUserByIdWithTasksAsync(Guid? uid);
    Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<LoginUserResponseDto> LoginAsync(LoginUserDto dto);
}