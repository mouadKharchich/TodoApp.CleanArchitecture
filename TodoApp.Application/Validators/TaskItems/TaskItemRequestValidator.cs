using FluentValidation;
using TodoApp.Application.Dtos.TaskItem;
using TodoApp.Domain.Enums;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace TodoApp.Application.Validators.TaskItems;

public class TaskItemRequestValidator : AbstractValidator<TaskItemRequestDto>
{
    public TaskItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => (int)x.Status)
            .InclusiveBetween(1, 4).WithMessage("Status must be between 1 and 4");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => (int)x.Priority)
            .InclusiveBetween(1, 3).WithMessage("Priority must be between 1 and 3");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Deadline is required")
            .Must(BeAValidDate).WithMessage("Deadline must be a valid date");

        RuleFor(x => x.UserId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("UserId cannot be empty when provided");
    }
    
    private bool BeAValidDate(DateTime date)
    {
        // Optionally, reject past dates:
        return date >= DateTime.Today;
    }
}