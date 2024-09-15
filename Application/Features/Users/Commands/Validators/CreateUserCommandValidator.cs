
using FluentValidation;

namespace Application.Features.Users.Commands.Validators;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} was not empty")
            .EmailAddress().WithMessage("{PropertyName} must be a valid email");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} was not empty");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}