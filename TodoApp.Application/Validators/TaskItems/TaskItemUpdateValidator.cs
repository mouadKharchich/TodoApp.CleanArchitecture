using FluentValidation;
using TodoApp.Application.Dtos.TaskItem;

namespace TodoApp.Application.Validators.TaskItems;

public class TaskItemUpdateValidator : AbstractValidator<TaskItemUpdateDto>
{
    public  TaskItemUpdateValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Deadline)
            .Must(BeAValidDate)
            .When(x => x.Deadline.HasValue)
            .WithMessage("Deadline must be a valid date");
    }
    
    private bool BeAValidDate(DateTime? date)
    {
        //  invalid or past dates 
        return date == null || date.Value >= DateTime.Today;
    }
}