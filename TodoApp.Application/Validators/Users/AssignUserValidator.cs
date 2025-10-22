using FluentValidation;
using TodoApp.Application.Dtos.User;

namespace TodoApp.Application.Validators.Users;

public class AssignUserValidator : AbstractValidator<AssignUserDto>
{
    public AssignUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}