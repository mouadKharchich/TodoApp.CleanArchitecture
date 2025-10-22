using FluentValidation;
using TodoApp.Application.Dtos.TaskItem;

namespace TodoApp.Application.Validators.TaskItems;

public class UpdatePriorityValidator : AbstractValidator<UpdatePriorityDto>
{
    public UpdatePriorityValidator()
    {
        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 and 3");
    }
}