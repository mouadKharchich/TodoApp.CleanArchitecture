using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.IRepositories;

public interface IAssignmentRepository
{
    Task<IEnumerable<Assignment>> GetAllAsignmentsAsync();
    Task<Assignment> GetAsignmentByIdAsync(Guid asignmentId);
    Task<IEnumerable<Assignment>> GetAsignmentsByTaskIdAsync(Guid taskItemId);
    Task<Assignment> AddAsignmentAsync(Assignment assignment);
}