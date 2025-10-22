using TodoApp.Domain.Enums;

namespace TodoApp.Application.Dtos.Assignment;

public class AssignmentResponseDto
{
    public Guid AssignmentId { get; set; }
    public Guid TaskItemId { get; set; }
    public string TaskItemTitle { get; set; } = string.Empty;
    public PriorityLevel TaskPriority { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}