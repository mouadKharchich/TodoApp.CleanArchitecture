using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Infrastructure.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasData(
            new TaskItem { 
                TaskItemId = 1,
                PublicId = Guid.NewGuid(),
                UserId = 1, 
                Title = "Buy groceries", 
                Description = "Buy milk, eggs, and bread", 
                Deadline = DateTime.Now.AddDays(1), 
                Status = TaskStatus.Pending, 
                Priority = PriorityLevel.Medium, 
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now,
                Assignments = new List<Assignment>()
            }
          );
    }
}