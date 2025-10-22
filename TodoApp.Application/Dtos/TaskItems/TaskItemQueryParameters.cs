using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Application.Dtos.TaskItem;

public class TaskItemQueryParameters
{
    public int PageNumber { get; set; } = 1; 
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public TaskStatus? Status { get; set; }
    public PriorityLevel? Priority { get; set; }
}