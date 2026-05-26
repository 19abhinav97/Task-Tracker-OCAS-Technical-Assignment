using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

// Data Transfer Object for creating a new task.
public class CreateTaskDto
{
    // Short summary of the task. Required, max 100 characters
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    // Optional longer description
    public string? Description { get; set; }

    // Initial status. Defaults to TaskStatus.Todo when omitted
    public TaskStatus? Status { get; set; }

    // Optional deadline
    public DateTime? DueDate { get; set; }
}
