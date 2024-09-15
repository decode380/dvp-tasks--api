using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Middlewares;

namespace Webapi;
public static class AppExtensions
{
    public static void AddAppMiddlewares(this IApplicationBuilder app){
        app.UseMiddleware<ErrorMiddleware>();
    }
}