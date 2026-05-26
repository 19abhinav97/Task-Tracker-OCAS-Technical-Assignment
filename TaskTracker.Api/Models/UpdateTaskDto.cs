using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models;

// Data Transfer Object for updating an existing task.
public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public TaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}
