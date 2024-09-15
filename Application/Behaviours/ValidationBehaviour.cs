using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Application.Behaviours;
public class ValidationBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator){
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken){
    var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new Application.Models.Exceptions.ValidationException(validationResult.Errors); 

        return await next();
    } 
}