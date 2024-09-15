using FluentValidation;

namespace Application.Features.Tasks.Queries.Validators;

public class GetTasksByUserQueryValidator : AbstractValidator<GetTasksByUserQuery>
{
    public GetTasksByUserQueryValidator()
    {
        RuleFor(p => p.PageNumber).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.PageSize).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.UserId).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}