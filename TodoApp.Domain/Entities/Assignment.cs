using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Domain.Entities;

public class Assignment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AssignmentId { get; set; }
    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();
    // Task Relation
    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    // User Relation
    public int? UserId { get; set; }
    public User? User { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}