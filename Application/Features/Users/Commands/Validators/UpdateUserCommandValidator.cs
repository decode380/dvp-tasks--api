
using FluentValidation;

namespace Application.Features.Users.Commands.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{PropertyName} was not empty");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}