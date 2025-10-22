using TodoApp.Application.Dtos.TaskItem;

namespace TodoApp.Application.Dtos.User;

public class UserWithTaskItemsDto:UserResponseDto
{
    public List<TaskItemResponseDto> TaskItems { get; set; } = new();
}