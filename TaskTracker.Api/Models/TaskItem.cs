using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

// The core entity representing a tracked task, persisted via EF Core.
public class TaskItem
{
    // Primary key (auto-increment).
    public int Id { get; set; }

    // Short summary of the task. Required, max 100 characters.
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    // Optional longer description of the task.
    public string? Description { get; set; }

    // Current lifecycle status. Defaults to TaskStatus.Todo.
    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    // Optional deadline for the task.
    public DateTime? DueDate { get; set; }

    // UTC timestamp set automatically when the task is first created.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
