using System.ComponentModel.DataAnnotations;
using Moq;
using TaskTracker.Api.Models;
using TaskTracker.Api.Models.Exceptions;
using TaskTracker.Api.Repositories;
using TaskTracker.Api.Services;

namespace TaskTracker.Tests;

/// <summary>
/// Unit tests for TaskService
/// The repository is mocked so tests run without a real database.
/// </summary>
public class TaskServiceTests
{
    private static TaskService CreateService(Mock<ITaskRepository> repoMock)
        => new TaskService(repoMock.Object);

    private static TaskItem SampleTask(string title = "Buy groceries") => new TaskItem
    {
        Id          = 1,
        Title       = title,
        Description = "Milk and eggs",
        Status      = TaskTracker.Api.Models.TaskStatus.Todo,
        CreatedAt   = DateTime.UtcNow
    };

    // 1. CreateTask — happy path

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var dto = new CreateTaskDto
        {
            Title       = "Buy groceries",
            Description = "Milk and eggs",
            DueDate     = DateTime.UtcNow.AddDays(1)
        };

        var expectedTask = SampleTask(dto.Title);
        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(expectedTask);

        var service = CreateService(repoMock);

        // Act
        var result = await service.CreateTaskAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(TaskTracker.Api.Models.TaskStatus.Todo, result.Status);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    // 2. CreateTask — empty title validation

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateTask_WithEmptyTitle_ThrowsValidationException(string? title)
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var dto = new CreateTaskDto { Title = title! };
        var service = CreateService(repoMock);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTaskAsync(dto));
        repoMock.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    // 3. UpdateTask — set to Done with valid title (happy path)

    [Fact]
    public async Task UpdateTask_SetStatusToDone_WithValidTitle_Succeeds()
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var existingTask = SampleTask("Buy groceries");

        var dto = new UpdateTaskDto
        {
            Title  = "Buy groceries",
            Status = TaskTracker.Api.Models.TaskStatus.Done
        };

        var updatedTask = SampleTask(dto.Title);
        updatedTask.Status = TaskTracker.Api.Models.TaskStatus.Done;

        repoMock
            .Setup(r => r.GetByIdAsync(existingTask.Id))
            .ReturnsAsync(existingTask);
        repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(updatedTask);

        var service = CreateService(repoMock);

        // Act
        var result = await service.UpdateTaskAsync(existingTask.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TaskTracker.Api.Models.TaskStatus.Done, result.Status);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    // 4. UpdateTask — set to Done with empty title
    // The general title-validation guard fires before the business-rule check,
    // so ValidationException is thrown. Behaviour from the caller's side (400) is identical.

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateTask_SetStatusToDone_WithEmptyTitle_ThrowsValidationException(string title)
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var existingTask = SampleTask("Original title");

        var dto = new UpdateTaskDto
        {
            Title  = title,
            Status = TaskTracker.Api.Models.TaskStatus.Done
        };

        repoMock
            .Setup(r => r.GetByIdAsync(existingTask.Id))
            .ReturnsAsync(existingTask);

        var service = CreateService(repoMock);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.UpdateTaskAsync(existingTask.Id, dto));

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    // 5. CreateTask — invalid status value

    [Fact]
    public async Task CreateTask_WithInvalidStatus_ThrowsValidationException()
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var dto = new CreateTaskDto
        {
            Title  = "Valid title",
            Status = (TaskTracker.Api.Models.TaskStatus)99
        };
        var service = CreateService(repoMock);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateTaskAsync(dto));
        repoMock.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    // 6. UpdateTask — invalid status value

    [Fact]
    public async Task UpdateTask_WithInvalidStatus_ThrowsValidationException()
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var existingTask = SampleTask("Buy groceries");

        var dto = new UpdateTaskDto
        {
            Title  = "Buy groceries",
            Status = (TaskTracker.Api.Models.TaskStatus)99
        };

        repoMock
            .Setup(r => r.GetByIdAsync(existingTask.Id))
            .ReturnsAsync(existingTask);

        var service = CreateService(repoMock);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.UpdateTaskAsync(existingTask.Id, dto));

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    // 7. UpdateTask — whitespace-only title is rejected for any status

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateTask_WithWhitespaceTitle_ThrowsValidationException(string title)
    {
        // Arrange
        var repoMock = new Mock<ITaskRepository>();
        var existingTask = SampleTask("Original title");

        var dto = new UpdateTaskDto
        {
            Title  = title,
            Status = TaskTracker.Api.Models.TaskStatus.Todo
        };
        
        repoMock
            .Setup(r => r.GetByIdAsync(existingTask.Id))
            .ReturnsAsync(existingTask);

        var service = CreateService(repoMock);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => service.UpdateTaskAsync(existingTask.Id, dto));

        repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }
}
