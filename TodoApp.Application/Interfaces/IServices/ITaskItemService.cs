using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Application.Interfaces.IServices;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllTasksAsync();
    Task<PaginatedTaskItemResponse> GetTasksByQueryAsync(TaskItemQueryParameters queryParameters);
    Task<TaskItemResponseDto?> GetTaskByIdAsync(Guid id);
    Task<TaskItemResponseDto> AddTaskAsync(TaskItemRequestDto taskItem);
    Task<TaskItemResponseDto> UpdateTaskAsync(Guid? id,TaskItemUpdateDto taskItem);
    Task DeleteTaskAsync(Guid id);
    Task<TaskItemResponseDto> AssignUserToTaskAsync(Guid taskId, Guid userId);
    Task<TaskItemResponseDto> UpdatePriorityAsync(Guid taskId, PriorityLevel priority);
    Task<TaskItemResponseDto> UpdateStatusAsync(Guid taskId, TaskStatus status);
}