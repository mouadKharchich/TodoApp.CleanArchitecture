namespace TodoApp.Application.Dtos.User;

public class LoginUserResponseDto
{
    public string? AcessToken { get; set; }
    public string? Username { get; set; }
    public int ExpiresIn { get; set; }
}