using FluentValidation;
using TodoApp.Application.Dtos.User;

namespace TodoApp.Application.Validators.Auths;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        Include(new LoginUserValidator());

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters");
    }
}