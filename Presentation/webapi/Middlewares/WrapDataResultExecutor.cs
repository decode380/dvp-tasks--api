
using Application.Models.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace Webapi.Middlewares;

public class WrapDataResultExecutor : ObjectResultExecutor
{
    public WrapDataResultExecutor(OutputFormatterSelector formatterSelector, IHttpResponseStreamWriterFactory writerFactory, ILoggerFactory loggerFactory, IOptions<MvcOptions> mvcOptions) : base(formatterSelector, writerFactory, loggerFactory, mvcOptions)
    {
    }

    public override Task ExecuteAsync(ActionContext context, ObjectResult result)
    {
        var response = new ResponseWrapper<dynamic>(result.Value, "Success");
        result.Value = response;

        return base.ExecuteAsync(context, result);
        
    }
}