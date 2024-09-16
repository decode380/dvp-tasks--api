
using FluentValidation;

namespace Application.Features.Tasks.Commands.Validators;

public class UpdateTaskNameOrAssignationCommandValidator: AbstractValidator<UpdateTaskNameOrAssignationCommand>
{
    public UpdateTaskNameOrAssignationCommandValidator()
    {
        RuleFor(p => p.TaskId).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.UserEmail).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.Name).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}