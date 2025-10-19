using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistences;

namespace TodoApp.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _dbContext;
    private ITaskItemRepository _taskItemRepositoryImplementation;

    public TaskItemRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Public Methods

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        try
        {
            return await _dbContext.TaskItems
                .Include(t => t.User)
                .Include(t => t.Assignments)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all task items: {ex.Message}", ex);
        }
    }

    public IQueryable<TaskItem> GetAllTasksQuery()
    {
        try
        {
            return _dbContext.TaskItems
                .Include(t => t.User)
                .Include(t => t.Assignments)
                .AsQueryable();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all task items as query: {ex.Message}", ex);
        }
    }

    public async Task<TaskItem> GetTaskByIdAsync(Guid? taskId)
    {
        try
        {
            var task = await _dbContext.TaskItems
                .Include(t => t.User)
                .Include(t => t.Assignments)
                .FirstOrDefaultAsync(t => t.PublicId == taskId);

            if (task == null)
                throw new NotFoundException($"Task with ID {taskId} not found.");

            return task;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching task with ID {taskId}: {ex.Message}", ex);
        }
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem taskItem)
    {
        try
        {
            _dbContext.TaskItems.Add(taskItem);
            await _dbContext.SaveChangesAsync();

            return await GetTaskByIdAsync(taskItem.PublicId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error creating new task: {ex.Message}", ex);
        }
    }

    public async Task UpdateTaskAsync(TaskItem taskItem)
    {
        try
        {
            await EnsureTaskExistsAsync(taskItem.TaskItemId);

            _dbContext.TaskItems.Update(taskItem);
            await _dbContext.SaveChangesAsync();
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error updating task with ID {taskItem.TaskItemId}: {ex.Message}", ex);
        }
    }

    public async Task DeleteTaskAsync(Guid taskId)
    {
        try
        {
            var taskItem = await GetTaskByIdAsync(taskId); // already throws if not found
            _dbContext.TaskItems.Remove(taskItem);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error deleting task with ID {taskId}: {ex.Message}", ex);
        }
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Checks if a task exists by internal TaskItemId. Throws NotFoundException if not.
    /// </summary>
    private async Task EnsureTaskExistsAsync(int taskItemId)
    {
        var exists = await _dbContext.TaskItems.AnyAsync(t => t.TaskItemId == taskItemId);
        if (!exists)
            throw new NotFoundException($"Task with ID {taskItemId} not found.");
    }

    #endregion
}
