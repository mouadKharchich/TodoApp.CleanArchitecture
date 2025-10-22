using FluentAssertions;
using Moq;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Interfaces.IRepositories;
using TodoApp.Application.Services;
using TodoApp.Domain.Entities;
using Xunit;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Tests.Services
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ITaskItemRepository> _taskRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IAssignmentRepository> _assignmentRepoMock;
        private readonly TaskItemService _service;
        private readonly Mock<IDatabaseTransaction> _mockTransactionMock;

        public TaskItemServiceTests()
        {
            _taskRepoMock = new Mock<ITaskItemRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _assignmentRepoMock = new Mock<IAssignmentRepository>();
            _mockTransactionMock = new Mock<IDatabaseTransaction>();
            _service = new TaskItemService(
                _taskRepoMock.Object,
                _userRepoMock.Object,
                _assignmentRepoMock.Object,
                _mockTransactionMock.Object
            );
        }

        // ----------------------------
        // TEST: GetTaskByIdAsync()
        // ----------------------------
        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnDto_WhenTaskExists()
        {
            // Arrange
            var publicTaskId = Guid.NewGuid();
            var taskEntity = new TaskItem
            {
                TaskItemId = 1,
                PublicId = publicTaskId,
                Title = "Test Task",
                Description = "Demo",
                Status = TaskStatus.Pending
            };

            _taskRepoMock.Setup(r => r.GetTaskByIdAsync(publicTaskId))
                .ReturnsAsync(taskEntity);

            // Act
            var result = await _service.GetTaskByIdAsync(publicTaskId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Task");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldThrowNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetTaskByIdAsync(id))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = async () => await _service.GetTaskByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{id}*");
        }

        // ----------------------------
        // TEST:AddTaskAsync()
        // ----------------------------
        [Fact]
        public async Task AddTaskAsync_ShouldCreateTaskAndAssignment_WhenUserExists()
        {
            // Arrange
            var userPublicId = Guid.NewGuid();
            var userEntity = new User { UserId = 1, PublicId = userPublicId };

            var taskDto = new TaskItemRequestDto
            {
                Title = "New Task",
                Description = "Desc",
                UserId = userPublicId
            };

            var taskEntity = new TaskItem
            {
                TaskItemId = 10,
                PublicId = Guid.NewGuid(),
                Title = taskDto.Title,
                Description = taskDto.Description
            };

            var returnedTaskEntity = new TaskItem
            {
                TaskItemId = 10,
                PublicId = taskEntity.PublicId,
                Title = taskDto.Title,
                Description = taskDto.Description
            };

            // Mock repository behavior
            _taskRepoMock
                .Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);

            _userRepoMock
                .Setup(r => r.GetUserByIdAsync(userPublicId))
                .ReturnsAsync(userEntity);

            _assignmentRepoMock
                .Setup(r => r.AddAssignmentAsync(It.IsAny<Assignment>()))
                .Returns(Task.CompletedTask);

            _taskRepoMock
                .Setup(r => r.GetTaskByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(returnedTaskEntity);

            _mockTransactionMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddTaskAsync(taskDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Task");

            _taskRepoMock.Verify(r => r.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
            _assignmentRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<Assignment>()), Times.Once);
            _mockTransactionMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task AddTaskAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var dto = new TaskItemRequestDto
            {
                Title = "Task",
                UserId = Guid.NewGuid()
            };

            _taskRepoMock.Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>()));
            _userRepoMock.Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _service.AddTaskAsync(dto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("*User*not found*");
        }

        // ----------------------------
        // TEST: DeleteTaskAsync()
        // ----------------------------
        [Fact]
        public async Task DeleteTaskAsync_ShouldDelete_WhenTaskExists()
        {
            // Arrange
            var publicTaskId = Guid.NewGuid();
            var task = new TaskItem { TaskItemId = 1, PublicId = publicTaskId};
            _taskRepoMock.Setup(r => r.GetTaskByIdAsync(publicTaskId)).ReturnsAsync(task);

            // Act
            await _service.DeleteTaskAsync(publicTaskId);

            // Assert
            _taskRepoMock.Verify(r => r.DeleteTaskAsync(publicTaskId), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldThrow_WhenTaskNotFound()
        {
            var taskId = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            Func<Task> act = async () => await _service.DeleteTaskAsync(taskId);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        // ----------------------------
        // AssignUserToTaskAsync()
        // ----------------------------
        [Fact]
        public async Task AssignUserToTaskAsync_ShouldAssignUserAndAddHistory()
        {
            // Arrange
            var publicTaskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = new TaskItem { TaskItemId = 1,PublicId = publicTaskId };
            var user = new User { UserId = 5, PublicId = userId };

            _taskRepoMock.Setup(r => r.GetTaskByIdAsync(publicTaskId)).ReturnsAsync(task);
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.AssignUserToTaskAsync(publicTaskId, userId);

            // Assert
            result.Should().NotBeNull();
            _taskRepoMock.Verify(r => r.UpdateTaskAsync(It.Is<TaskItem>(t => t.UserId == 5)), Times.Once);
            _assignmentRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<Assignment>()), Times.Once);
        }
    }
}
