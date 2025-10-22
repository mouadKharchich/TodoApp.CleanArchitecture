using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Dtos.Assignment;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Application.Mappings;

namespace TodoApp.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }
    
    /// <summary>
    /// Retrieves all assignments from the database.
    /// </summary>
    public async Task<IEnumerable<AssignmentResponseDto>> GetAllAssignmentsAsync()
    {
        var assignments = await _assignmentRepository.GetAllAssignmentsAsync();
        return assignments.Select(AssignmentMapper.ToDto);
    }

    /// <summary>
    /// Retrieves a specific assignment by taskItemId.
    /// </summary>
    public async Task<IEnumerable<AssignmentResponseDto>> GetAssignmentsByTaskIdAsync(Guid? taskItemId)
    {
        if (!taskItemId.HasValue)
        {
            throw new ArgumentException("Task ID cannot be null or empty.");
        }
        var assignments = await _assignmentRepository.GetAssignmentsByTaskIdAsync(taskItemId);
        return assignments.Select(AssignmentMapper.ToDto);
    }

    /// <summary>
    /// Retrieves a specific assignment by assignmentId.
    /// </summary>
    public async Task<AssignmentResponseDto> GetAssignmentsByIdAsync(Guid? assignmentId)
    {
        if (!assignmentId.HasValue)
        {
            throw new ArgumentException("Assignment ID cannot be null or empty.");
        }
        var assignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        if (assignment is null)
        {
            throw new NotFoundException($"Task Item with ID {assignmentId} not found");
        }
        return AssignmentMapper.ToDto(assignment);
    }
}