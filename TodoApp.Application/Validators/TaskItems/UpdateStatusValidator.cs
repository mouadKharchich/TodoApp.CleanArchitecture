using FluentValidation;
using TodoApp.Application.Dtos.TaskItem;

namespace TodoApp.Application.Validators.TaskItems;

public class UpdateStatusValidator : AbstractValidator<UpdateStatusDto>
{
    public UpdateStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .InclusiveBetween(1, 4).WithMessage("Status must be between 1 and 4");
    }
}