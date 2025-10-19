using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.Dtos.TaskItem;

public class UpdateStatusDto
{
    [Required(ErrorMessage="Status is required")]
    [Range(1, 4, ErrorMessage="Status must be between 1 and 4")]
    public int Status { get; set; }
}