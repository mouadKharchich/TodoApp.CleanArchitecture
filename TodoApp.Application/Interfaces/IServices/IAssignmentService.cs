using TodoApp.Application.Dtos.Assignment;

namespace TodoApp.Application.Interfaces.IServices;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentResponseDto>> GetAllAssignmentsAsync();
    Task<IEnumerable<AssignmentResponseDto>> GetAssignmentsByTaskIdAsync(Guid? taskItemId);
    Task<AssignmentResponseDto> GetAssignmentsByIdAsync(Guid? assignmentId);
}