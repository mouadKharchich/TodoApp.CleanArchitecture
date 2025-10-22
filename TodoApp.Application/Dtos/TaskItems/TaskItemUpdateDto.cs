using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Application.Dtos.TaskItem;

public class TaskItemUpdateDto
{ 
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}