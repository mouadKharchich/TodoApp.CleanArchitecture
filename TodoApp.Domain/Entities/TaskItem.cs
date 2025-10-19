using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoApp.Domain.Enums;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace TodoApp.Domain.Entities;

public class TaskItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaskItemId { get; set; }
    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();
    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Enums.TaskStatus Status { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // User Relation
    public int? UserId { get; set; }
    public User? User { get; set; }
    
    // Assignment Relation
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}