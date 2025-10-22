using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;

namespace TodoApp.Infrastructure.Persistences;

public class DatabaseTransaction : IDatabaseTransaction
{
    private readonly AppDbContext  _dbContext;

    public DatabaseTransaction(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// The repository should not decide when to persist changes — the service (business logic) should.
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}