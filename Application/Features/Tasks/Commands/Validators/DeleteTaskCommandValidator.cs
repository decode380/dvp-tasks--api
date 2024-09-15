
using FluentValidation;

namespace Application.Features.Tasks.Commands.Validators;

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(p => p.TaskId).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}