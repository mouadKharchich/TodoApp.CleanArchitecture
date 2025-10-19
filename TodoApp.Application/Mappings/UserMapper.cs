using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Application.Dtos.User;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings;

public class UserMapper
{
    public static User ToEntity(RegisterUserDto dto)
    {
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.Password,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            UserId = user.PublicId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static UserWithTaskItemsDto ToDtoWithTasks(User user)
    {
        return new UserWithTaskItemsDto
        {
            UserId = user.PublicId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TaskItems = user.Tasks?
                .Select(TaskItemMapper.ToDto)
                .ToList() ?? new List<TaskItemResponseDto>()
        };
    }
}