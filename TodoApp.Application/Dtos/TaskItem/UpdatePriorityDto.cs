using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Dtos.TaskItem;

public class UpdatePriorityDto
{
    [Required(ErrorMessage="Priority level is required")]
    [Range(1, 3, ErrorMessage="Priority must be between 1 and 3")]
    public int Priority { get; set; }
}