using TodoApp.Application.Dtos.Assignment;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Mappings;

public class AssignmentMapper
{
    public static AssignmentResponseDto ToDto(Assignment entity)
    {
        return new AssignmentResponseDto
        {
            AssignmentId = entity.PublicId,
            TaskItemId = entity.TaskItem?.PublicId ?? Guid.Empty,
            TaskItemTitle = entity.TaskItem?.Title ?? string.Empty,
            TaskPriority = entity.TaskItem?.Priority ?? PriorityLevel.None,
            UserId = entity.User?.PublicId ?? Guid.Empty,
            Username = entity.User?.Username ?? string.Empty,
            Email = entity.User?.Email ?? string.Empty
        };
    }
}