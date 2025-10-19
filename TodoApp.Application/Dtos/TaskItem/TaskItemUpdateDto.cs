using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Application.Dtos.TaskItem;

public class TaskItemUpdateDto
{
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string? Title { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [EnumDataType(typeof(TaskStatus), ErrorMessage = "Invalid status value")]
    [Range(1, 4, ErrorMessage="Status must be between 1 and 4")]
    public TaskStatus? Status { get; set; }
    
    [EnumDataType(typeof(PriorityLevel), ErrorMessage = "Invalid priority value")]
    [Range(1, 3, ErrorMessage="Priority must be between 1 and 3")]
    public PriorityLevel? Priority { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? Deadline { get; set; }
}