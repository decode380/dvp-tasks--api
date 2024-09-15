
using FluentValidation;

namespace Application.Features.Users.Commands.Validators;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}