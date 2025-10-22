
namespace TodoApp.Application.Dtos.User;

public class RegisterUserDto : LoginUserDto
{
    public string Username { get; set; } = string.Empty;
}