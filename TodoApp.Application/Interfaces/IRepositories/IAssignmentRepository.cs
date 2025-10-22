using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.IRepositories;

public interface IAssignmentRepository
{
    Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
    Task<Assignment?> GetAssignmentByIdAsync(Guid? assignmentId);
    Task<IEnumerable<Assignment>> GetAssignmentsByTaskIdAsync(Guid? taskItemId);
    Task AddAssignmentAsync(Assignment assignment);
}