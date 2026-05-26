using TaskTracker.Api.Models;

namespace TaskTracker.Api.Services;
public interface ITaskService
{
    // Returns every task in the system
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();

    // Returns the task with the given id or null if not found
    Task<TaskItem?> GetTaskByIdAsync(int id);

    // Creates a new task from the supplied DTO and returns the entity
    Task<TaskItem> CreateTaskAsync(CreateTaskDto dto);

    // Updates the task identified by id with values from dto
    // Returns the updated entity, or null if the task does not exist.
    Task<TaskItem?> UpdateTaskAsync(int id, UpdateTaskDto dto);

    // Deletes the task with the given id.
    // Returns true if deleted, false if the task was not found.
    Task<bool> DeleteTaskAsync(int id);
}
