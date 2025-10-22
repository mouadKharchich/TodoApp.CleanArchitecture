namespace TodoApp.Application.Interfaces;

public interface IDatabaseTransaction
{
    Task<int> SaveChangesAsync();
}