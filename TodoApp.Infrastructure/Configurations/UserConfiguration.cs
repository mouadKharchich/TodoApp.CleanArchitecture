using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            new User { 
                UserId = 1, 
                PublicId = Guid.NewGuid(),
                Username = "admin", 
                Email = "admin@gmail.com", 
                PasswordHash = "$2a$11$XgEpzAUpRBXagENMz/FDiO5ZALiNp9f3dByo.msTUpeKFodM6Rv/K",//admin123
                CreatedAt = DateTime.Now, 
                UpdatedAt = DateTime.Now, 
                Tasks = new List<TaskItem>(),
                Assignments = new List<Assignment>(),
            }
        );
    }
}