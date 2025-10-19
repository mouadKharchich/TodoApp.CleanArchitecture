using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.IRepositories;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    IQueryable<TaskItem> GetAllTasksQuery();
    Task<TaskItem?> GetTaskByIdAsync(Guid? taskId);
    Task<TaskItem> CreateTaskAsync(TaskItem taskItem);
    Task UpdateTaskAsync(TaskItem taskItem);
    Task DeleteTaskAsync(Guid taskId);
}