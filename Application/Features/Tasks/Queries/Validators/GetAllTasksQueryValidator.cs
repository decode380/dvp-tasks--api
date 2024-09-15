
using FluentValidation;

namespace Application.Features.Tasks.Queries.Validators;

public class GetAllTasksQueryValidator : AbstractValidator<GetAllTasksQuery>
{
    public GetAllTasksQueryValidator()
    {
        RuleFor(p => p.PageNumber).NotEmpty().WithMessage("{PropertyName} was not empty");
        RuleFor(p => p.PageSize).NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}