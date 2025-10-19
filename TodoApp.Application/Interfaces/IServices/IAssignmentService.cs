using TodoApp.Application.Dtos.Assignment;

namespace TodoApp.Application.Interfaces.IServices;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentResponseDto>> GetAllAsignmentsAsync();
    Task<IEnumerable<AssignmentResponseDto>> GetAsignmentsByTaskIdAsync(Guid taskItemId);
    Task<AssignmentResponseDto> GetAsignmentsByIdAsync(Guid id);
}