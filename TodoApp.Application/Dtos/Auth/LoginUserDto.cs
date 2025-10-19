using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.Dtos.User;

public class LoginUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = String.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6,ErrorMessage="Min Length 6")]
    public string Password { get; set; } = string.Empty;
}