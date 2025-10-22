using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.Dtos.User;

public class AssignUserDto
{
    [Required(ErrorMessage="UserId is required")]
    public Guid UserId { get; set; }
}