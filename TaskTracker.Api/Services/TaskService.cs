using System.ComponentModel.DataAnnotations;
using TaskTracker.Api.Models;
using TaskTracker.Api.Models.Exceptions;
using TaskTracker.Api.Repositories;

namespace TaskTracker.Api.Services;

/// <summary>
/// Implements <see cref="ITaskService"/> with all domain business rules.
/// Delegates data access exclusively to <see cref="ITaskRepository"/>.
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    // Initialises the service with the injected repository dependency.
    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ValidationException("Title is required.");

        var task = new TaskItem
        {
            Title       = dto.Title.Trim(),
            Description = dto.Description,
            Status      = dto.Status ?? Models.TaskStatus.Todo,
            DueDate     = dto.DueDate,
            CreatedAt   = DateTime.UtcNow
        };

        return await _repository.CreateAsync(task);
    }

    public async Task<TaskItem?> UpdateTaskAsync(int id, UpdateTaskDto dto)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null)
            return null;

        // A task cannot be set to Done with an empty title.
        if (dto.Status == Models.TaskStatus.Done && string.IsNullOrWhiteSpace(dto.Title))
            throw new BusinessRuleException(
                "A task cannot be marked as Done when the title is empty or whitespace.");

        task.Title       = dto.Title.Trim();
        task.Description = dto.Description;
        task.Status      = dto.Status;
        task.DueDate     = dto.DueDate;

        return await _repository.UpdateAsync(task);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task is null)
            return false;

        await _repository.DeleteAsync(task);
        return true;
    }
}
