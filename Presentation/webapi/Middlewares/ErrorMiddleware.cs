

using System.Net;
using System.Text.Json;
using Application.Models.Wrappers;

namespace Webapi.Middlewares;

public class ErrorMiddleware
{
    private readonly ILoggerFactory _loggerFactory; 
    private readonly RequestDelegate _next;

    public ErrorMiddleware(ILoggerFactory loggerFactory, RequestDelegate next)
    {
      _loggerFactory = loggerFactory;
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var _logger = _loggerFactory.CreateLogger<ErrorMiddleware>();
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            ResponseWrapper responseModel = new("Something went wrong")
            {
                Succeeded = false,
                Errors = new List<string>(){error.Message}
            };

            switch (error)
            {
                case Application.Models.Exceptions.ValidationException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Errors = e.Errors;
                    break;
                case Application.Models.Exceptions.ApiException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(responseModel);
            _logger.LogError($"An error has occurred {DateTime.UtcNow}(UTC) :::: {error.Message} \n {error}");
            await response.WriteAsync(result);

        }
    }
}