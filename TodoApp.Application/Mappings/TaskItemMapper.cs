using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings;

public class TaskItemMapper
{
     public static TaskItem ToEntity(TaskItemRequestDto dto)
        {
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static TaskItemResponseDto ToDto(TaskItem entity)
        {
            return new TaskItemResponseDto
            {
                TaskItemId = entity.PublicId,
                Title = entity.Title,
                Description = entity.Description,
                Status = entity.Status.ToString(),
                Priority = entity.Priority.ToString(),
                Deadline = entity.Deadline,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                UserId = entity.User?.PublicId ?? Guid.Empty,
                Username = entity.User?.Username ?? string.Empty,
                Email = entity.User?.Email ?? string.Empty,
                Assignments = entity.Assignments.Select(AssignmentMapper.ToDto)
            };
        }

        /// <summary>
        /// Returns a new updated TaskItem entity from an existing entity and TaskUpdateDto.
        /// Only non-null fields from the DTO are applied.
        /// </summary>
        public static void ToUpdatedEntity(TaskItem entity, TaskItemUpdateDto dto)
        {
            entity.Title = dto.Title ?? entity.Title;
            entity.Description = dto.Description ?? entity.Description;
            entity.Deadline = dto.Deadline ?? entity.Deadline;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        
}