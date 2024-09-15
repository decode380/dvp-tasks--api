
using System.Net;
using Application.Models.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Webapi.Middlewares;

namespace Webapi;

public static class ServiceExtensions
{
    public static void AddPresentationLayer(this IServiceCollection services) {
        services.AddSingleton<IActionResultExecutor<ObjectResult>, WrapDataResultExecutor>();
        services.AddMvc(o => {
            o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResponseWrapper<object>), (int)HttpStatusCode.BadRequest));
            o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResponseWrapper<object>), (int)HttpStatusCode.InternalServerError));
        });
    }
}