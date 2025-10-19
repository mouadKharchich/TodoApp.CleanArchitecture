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
    public async Task<IEnumerable<AssignmentResponseDto>> GetAllAsignmentsAsync()
    {
        var assignments = await _assignmentRepository.GetAllAsignmentsAsync();
        return assignments.Select(AssignmentMapper.ToDto);
    }

    public async Task<IEnumerable<AssignmentResponseDto>> GetAsignmentsByTaskIdAsync(Guid taskItemId)
    {
        var assignments = await _assignmentRepository.GetAsignmentsByTaskIdAsync(taskItemId);
        return assignments.Select(AssignmentMapper.ToDto);
    }

    public async Task<AssignmentResponseDto> GetAsignmentsByIdAsync(Guid asignmentId)
    {
        var assignment = await _assignmentRepository.GetAsignmentByIdAsync(asignmentId);
        if (assignment == null)
            throw new NotFoundException($"Task Item with ID {asignmentId} not found");
        return AssignmentMapper.ToDto(assignment);
    }
}