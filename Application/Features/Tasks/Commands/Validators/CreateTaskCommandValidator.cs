
using FluentValidation;

namespace Application.Features.Tasks.Commands.Validators;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.StateId).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.UserEmail).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}