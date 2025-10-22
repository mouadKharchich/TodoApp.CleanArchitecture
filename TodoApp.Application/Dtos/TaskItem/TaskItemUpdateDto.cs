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
    
    [DataType(DataType.Date)]
    public DateTime? Deadline { get; set; }
}