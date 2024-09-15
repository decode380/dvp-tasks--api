
using FluentValidation;

namespace Application.Features.Auth.Queries.Validators;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} was not empty");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("{PropertyName} was not empty");
    }
}