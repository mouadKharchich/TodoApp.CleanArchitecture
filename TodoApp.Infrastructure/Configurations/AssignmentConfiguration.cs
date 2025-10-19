using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configurations;

public class AssignmentConfiguration: IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        // Seed data
        builder.HasData(
            new Assignment(){ 
                AssignmentId = 1,
                PublicId = Guid.NewGuid(),
                TaskItemId = 1, 
                UserId = 1,
                AssignedAt = DateTime.Now
            }
        );
    }
}