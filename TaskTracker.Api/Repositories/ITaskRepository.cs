using TaskTracker.Api.Models;

namespace TaskTracker.Api.Repositories;

// Data-access contract for TaskItem persistence
public interface ITaskRepository
{
    // Returns all tasks from the data store
    Task<IEnumerable<TaskItem>> GetAllAsync();

    // Returns the task with the given id,
    // or null if no such task exists
    Task<TaskItem?> GetByIdAsync(int id);

    // Persists a new task and returns the saved entity with its assigned id
    Task<TaskItem> CreateAsync(TaskItem task);

    // Persists changes to an existing task and returns the updated entity.
    Task<TaskItem> UpdateAsync(TaskItem task);

    // Removes (Hard deletes) the given task from the data store.
    Task DeleteAsync(TaskItem task);
}
