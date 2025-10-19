using Moq;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Interfaces.IServices;
using TodoApp.Application.Services;
using TodoApp.Domain.Entities;
using Xunit;

namespace TodoApp.Application.Tests;

public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
    private readonly IAssignmentService _assignmentService;

    public AssignmentServiceTests()
    {
        _mockAssignmentRepository = new Mock<IAssignmentRepository>();
        _assignmentService = new AssignmentService(_mockAssignmentRepository.Object);
    }

    [Fact]
    public async Task GetAllAssignmentsAsync_ShouldReturnAllAssignments()
    {
        // Arrange
        var mockAssignments = new List<Assignment>
        {
            new Assignment { AssignmentId = 1,PublicId = Guid.NewGuid(),AssignedAt = DateTime.UtcNow,UserId = 1,User = new User(),TaskItemId = 1,TaskItem = new TaskItem()},
            new Assignment { AssignmentId = 2,PublicId = Guid.NewGuid(),AssignedAt = DateTime.UtcNow,UserId = 2,User = new User(),TaskItemId = 2,TaskItem = new TaskItem()}
        };

        _mockAssignmentRepository
            .Setup(r => r.GetAllAsignmentsAsync())
            .ReturnsAsync(mockAssignments);

        // Act 
        var result = await _assignmentService.GetAllAsignmentsAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task GetAssignmentsByTaskIdAsync_ShouldReturnAssignmentsForTaskItem()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var mockAssignments = new List<Assignment>
        {
            new Assignment { AssignmentId = 1,
                PublicId = Guid.NewGuid(),
                AssignedAt = DateTime.UtcNow,
                UserId = 1,
                User = new User(),
                TaskItemId = 1,
                TaskItem = new TaskItem
                {
                    TaskItemId = 1,
                    PublicId = Guid.NewGuid(),
                    Title = "Task Item 1"
                }},
        };

        _mockAssignmentRepository
            .Setup(r => r.GetAsignmentsByTaskIdAsync(taskId))
            .ReturnsAsync(mockAssignments);

        // Act
        var result = await _assignmentService.GetAsignmentsByTaskIdAsync(taskId);

        // Assert
        Assert.Single(result);
        Assert.Equal("Task Item 1", result.FirstOrDefault().TaskItemTitle);
    }
    
    
    [Fact]
    public async Task GetAssignmentByIdAsync_ShouldReturnAssignment_WhenExists()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var mockAssignment = new Assignment { 
            AssignmentId = 1,
            PublicId = Guid.NewGuid(),
            AssignedAt = DateTime.UtcNow,
            UserId = 1,
            User = new User(),
            TaskItemId = 1,
            TaskItem = new TaskItem
            {
                TaskItemId = 1,
                PublicId = Guid.NewGuid(),
                Title = "Task Item 1"
            }};

        _mockAssignmentRepository
            .Setup(r => r.GetAsignmentByIdAsync(assignmentId))
            .ReturnsAsync(mockAssignment);

        // Act
        var result = await _assignmentService.GetAsignmentsByIdAsync(assignmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Task Item 1", result.TaskItemTitle);
    }
    
    [Fact]
    public async Task GetAssignmentByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        _mockAssignmentRepository
            .Setup(r => r.GetAsignmentByIdAsync(assignmentId))
            .ReturnsAsync((Assignment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _assignmentService.GetAsignmentsByIdAsync(assignmentId)
        );
    }
    
}