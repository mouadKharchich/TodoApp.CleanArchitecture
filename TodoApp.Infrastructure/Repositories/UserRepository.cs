using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistences;

namespace TodoApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Public Methods
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Tasks)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all users from database: {ex.Message}", ex);
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid? uid)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(u => u.Tasks)
                .ThenInclude(u => u.Assignments)
                .FirstOrDefaultAsync(u => u.PublicId == uid);

            return user;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching user with ID {uid} from database: {ex.Message}", ex);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower());
            
            return user;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching user with Email {email.Trim().ToLower()} from database: {ex.Message}", ex);
        }
    }
    
    public async Task CreateUserAsync(User user)
    {
        try
        {
            await _dbContext.Users.AddAsync(user);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error creating new user: {ex.Message}", ex);
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            await EnsureUserExistsAsync(user.PublicId);
            _dbContext.Users.Update(user);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RepositoryException($"Concurrency error while updating user {user.PublicId}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error updating user {user.PublicId}: {ex.Message}", ex);
        }
    }

    public async Task DeleteUserAsync(Guid uid)
    {
        try
        {
            var user = await GetUserByIdAsync(uid);
            _dbContext.Users.Remove(user!);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error deleting user with ID {uid}: {ex.Message}", ex);
        }
    }
    
    #endregion
    
    #region Private Helpers

    /// <summary>
    /// Checks if a task exists by internal TaskItemId. Throws NotFoundException if not.
    /// </summary>
    private async Task EnsureUserExistsAsync(Guid userId)
    {
        var exists = await _dbContext.Users.AnyAsync(t => t.PublicId == userId);
        if (!exists)
            throw new NotFoundException($"User with ID {userId} not found.");
    }

    #endregion
}
