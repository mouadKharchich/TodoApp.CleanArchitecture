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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            return await _dbContext.Users
                .Include(u => u.Tasks)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error fetching all users from database: {ex.Message}", ex);
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid uid)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.PublicId == uid);

            if (user == null)
                throw new NotFoundException($"User with ID {uid} not found.");

            return user;
        }
        catch (NotFoundException)
        {
            throw;
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
    
    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var createdUser = await GetUserByIdAsync(user.PublicId);
            return createdUser!;
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
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
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
            await _dbContext.SaveChangesAsync();
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Error deleting user with ID {uid}: {ex.Message}", ex);
        }
    }
}
