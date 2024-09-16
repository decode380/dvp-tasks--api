
using FluentValidation;

namespace Application.Features.Users.Queries.Validators;
public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}