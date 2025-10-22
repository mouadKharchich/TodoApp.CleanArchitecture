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

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync()
    {
        try
        {
            return await _dbContext.Assignments
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all assignments from database : {ex.Message}", ex);
        }
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(Guid? assignmentId)
    {
        try
        {
            var task = await _dbContext.Assignments
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .FirstOrDefaultAsync(t => t.PublicId == assignmentId);
            return task;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching Assignment with asignmentId {assignmentId} from database: {ex.Message}", ex);
        }
    }
    
    public async Task<IEnumerable<Assignment>> GetAssignmentsByTaskIdAsync(Guid? taskItemId)
    {
        try
        {
            var tasks = await _dbContext.Assignments
                .Include(t => t.User)
                .Include(t => t.TaskItem)
                .Where(t => t.TaskItem!=null && t.TaskItem.PublicId == taskItemId)
                .ToListAsync();

            return tasks;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching Assignment with TaskItemId {taskItemId} from database: {ex.Message}", ex);
        }
    }

    public async Task AddAssignmentAsync(Assignment assignment)
    {
        try
        {
           await _dbContext.Assignments.AddAsync(assignment);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error creating new assignment in database : {ex.Message}",ex);
        }
    }
}