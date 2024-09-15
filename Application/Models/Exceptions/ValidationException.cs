using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Application.Models.Exceptions;
public class ValidationException: Exception
{
    public List<string> Errors {get;} = default!;

    public ValidationException(): base("Se han producido uno o más errores de validación")
    {
        Errors = new List<string>();
    } 

    public ValidationException(IEnumerable<ValidationFailure> failures): this()
    {
        foreach (var failure in failures)
        {
            Errors.Add(failure.ErrorMessage); 
        } 
    }
}