
using FluentValidation;

namespace Application.Features.Tasks.Commands.Validators;

public class UpdateTaskStateCommandValidator : AbstractValidator<UpdateTaskStateCommand>
{
    public UpdateTaskStateCommandValidator()
    {
        RuleFor(p => p.TaskId).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.StateId).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}