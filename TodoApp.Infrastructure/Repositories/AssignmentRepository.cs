using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistences;

namespace TodoApp.Infrastructure.Repositories;

public class AssignmentRepository:IAssignmentRepository
{
    private readonly AppDbContext _dbContext;

    public AssignmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Assignment>> GetAllAsignmentsAsync()
    {
        try
        {
            return await _dbContext.Assignments
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all assignments from database : {ex.Message}", ex);
        }
    }

    public async Task<Assignment> GetAsignmentByIdAsync(Guid asignmentId)
    {
        try
        {
            var task = await _dbContext.Assignments
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .FirstOrDefaultAsync(t => t.PublicId == asignmentId);

            if (task == null)
                throw new NotFoundException($"Assignment with asignmentId {asignmentId} not found.");

            return task;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching Assignment with asignmentId {asignmentId} from database: {ex.Message}", ex);
        }
    }
    
    public async Task<IEnumerable<Assignment>> GetAsignmentsByTaskIdAsync(Guid taskItemasignmentId)
    {
        try
        {
            var tasks = await _dbContext.Assignments
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .Where(t => t.TaskItem!=null && t.TaskItem.PublicId == taskItemasignmentId)
                .ToListAsync();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching Assignment with TaskasignmentId {taskItemasignmentId} from database: {ex.Message}", ex);
        }
    }

    public async Task<Assignment> AddAsignmentAsync(Assignment assignment)
    {
        try
        {
            _dbContext.Assignments.Add(assignment);
            await _dbContext.SaveChangesAsync();
            return await GetAsignmentByIdAsync(assignment.PublicId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error creating new assignment in database : {ex.Message}",ex);
        }
    }
}