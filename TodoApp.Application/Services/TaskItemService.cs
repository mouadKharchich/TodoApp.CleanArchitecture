using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Application.Mappings;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Application.Services;

/// <summary>
/// Service layer for managing TaskItem operations.
/// This class encapsulates business logic and interacts with the repository layer.
/// </summary>
public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public TaskItemService(
        ITaskItemRepository taskItemRepository,
        IUserRepository userRepository,
        IAssignmentRepository assignmentRepository)
    {
        _taskItemRepository = taskItemRepository;
        _userRepository = userRepository;
        _assignmentRepository = assignmentRepository;
    }

    /// <summary>
    /// Retrieves all task items from the database.
    /// </summary>
    public async Task<IEnumerable<TaskItemResponseDto>> GetAllTasksAsync()
    {
        var taskItems = await _taskItemRepository.GetAllTasksAsync();
        return taskItems.Select(TaskItemMapper.ToDto);
    }

    /// <summary>
    /// Retrieves paginated and filtered task items based on query parameters.
    /// Supports search, filtering, and pagination.
    /// </summary>
    public async Task<PaginatedTaskItemResponse> GetTasksByQueryAsync(TaskItemQueryParameters queryParameters)
    {
        // Base query from repository
        var taskItemsQuery = _taskItemRepository.GetAllTasksQuery();

        // 🔍 Search filter: title or description
        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            taskItemsQuery = taskItemsQuery.Where(t =>
                t.Title.Contains(queryParameters.Search) ||
                t.Description.Contains(queryParameters.Search));
        }

        // Filter by status
        if (queryParameters.Status.HasValue)
        {
            taskItemsQuery = taskItemsQuery.Where(t => t.Status == queryParameters.Status.Value);
        }

        // Filter by priority
        if (queryParameters.Priority.HasValue)
        {
            taskItemsQuery = taskItemsQuery.Where(t => t.Priority == queryParameters.Priority.Value);
        }

        // Pagination calculation
        var totalCount = taskItemsQuery.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / queryParameters.PageSize);

        // Fetch current page items
        var taskItems = taskItemsQuery
            .OrderByDescending(t => t.Deadline)
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(TaskItemMapper.ToDto)
            .ToList();

        // Return paginated response
        return new PaginatedTaskItemResponse
        {
            TotalCount = totalCount,
            TotalPage = totalPages,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize,
            Items = taskItems
        };
    }

    /// <summary>
    /// Retrieves a specific task by ID.
    /// </summary>
    /// <param name="id">Task ID</param>
    public async Task<TaskItemResponseDto> GetTaskByIdAsync(Guid id)
    {
        var taskItem = await _taskItemRepository.GetTaskByIdAsync(id);
        if (taskItem == null)
            throw new NotFoundException($"Task Item with ID {id} not found");

        return TaskItemMapper.ToDto(taskItem);
    }

    /// <summary>
    /// Creates a new task item.
    /// </summary>
    public async Task<TaskItemResponseDto> AddTaskAsync(TaskItemRequestDto taskItemDto)
    {
        var taskItemEntity = TaskItemMapper.ToEntity(taskItemDto);
        var createdTask = await _taskItemRepository.CreateTaskAsync(taskItemEntity);
        
        // Get user by UserPublicId
        var user = await _userRepository.GetUserByIdAsync(taskItemDto.UserId ?? Guid.Empty)
                   ?? throw new NotFoundException($"User with Id {taskItemDto.UserId} not found");
        
        // Add History of Assignment To Track every change in assignment task
        var assignment = new Assignment() { TaskItemId = taskItemEntity.TaskItemId, UserId = user.UserId };
        await _assignmentRepository.AddAsignmentAsync(assignment);
        
        return TaskItemMapper.ToDto(createdTask);
    }

    /// <summary>
    /// Updates an existing task item.
    /// </summary>
    public async Task<TaskItemResponseDto> UpdateTaskAsync(Guid? taskId, TaskItemUpdateDto taskDto)
    {
        var taskEntity = await _taskItemRepository.GetTaskByIdAsync(taskId);
        if (taskEntity == null)
            throw new NotFoundException($"Task Item with ID {taskId} not found");

        TaskItemMapper.ToUpdatedEntity(taskEntity, taskDto);
        await _taskItemRepository.UpdateTaskAsync(taskEntity);

        return TaskItemMapper.ToDto(taskEntity);
    }

    /// <summary>
    /// Deletes a task item by ID.
    /// </summary>
    public async Task DeleteTaskAsync(Guid taskId)
    {
        var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
        if (task == null)
            throw new NotFoundException($"Task Item with ID {taskId} not found");

        await _taskItemRepository.DeleteTaskAsync(taskId);
    }

    /// <summary>
    /// Assigns a user to a specific task.
    /// </summary>
    public async Task<TaskItemResponseDto> AssignUserToTaskAsync(Guid taskId, Guid userId)
    {
        // fetch task & user 
        var task = await _taskItemRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException($"Task Item with ID {taskId} not found");

        var user = await _userRepository.GetUserByIdAsync(userId)
            ?? throw new NotFoundException($"User with Id {userId} not found");

        // update task
        task.UserId = user.UserId;
        await _taskItemRepository.UpdateTaskAsync(task);
        
        // Add History of Assignment To Track every change in assignment task
        var assignment = new Assignment() { TaskItemId = task.TaskItemId, UserId = user.UserId };
        await _assignmentRepository.AddAsignmentAsync(assignment);
        
        // return map task
        return TaskItemMapper.ToDto(task);
    }

    /// <summary>
    /// Updates the priority of a task.
    /// </summary>
    public async Task<TaskItemResponseDto> UpdatePriorityAsync(Guid taskId, PriorityLevel priority)
    {
        var task = await _taskItemRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException("Task Item not found");

        task.Priority = priority;
        await _taskItemRepository.UpdateTaskAsync(task);

        return TaskItemMapper.ToDto(task);
    }

    /// <summary>
    /// Updates the status of a task.
    /// </summary>
    public async Task<TaskItemResponseDto> UpdateStatusAsync(Guid taskId, TaskStatus status)
    {
        var task = await _taskItemRepository.GetTaskByIdAsync(taskId)
            ?? throw new NotFoundException("Task Item not found");

        task.Status = status;
        await _taskItemRepository.UpdateTaskAsync(task);

        return TaskItemMapper.ToDto(task);
    }
}
