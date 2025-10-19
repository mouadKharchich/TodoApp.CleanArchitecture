using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApp.Infrastructure.Persistences;

public static class AppDbContextFactory
{
    public static void AddInMemoryDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TodoAppDB"));
    }
}