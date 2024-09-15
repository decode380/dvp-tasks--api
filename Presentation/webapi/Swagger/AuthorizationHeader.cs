using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Webapi.Swagger;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizationHeader : Attribute, IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        CustomAttribute authorizationHeader = context.RequireAttribute<AuthorizationHeader>();
        if (!authorizationHeader.ContainsAttribute)
            return;

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "authorization",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema() { Type = "string" }
        });
    }
}


public class CustomAttribute
{
    public readonly bool ContainsAttribute;
    public readonly bool Mandatory;

    public CustomAttribute(bool containsAttribute, bool mandatory)
    {
        ContainsAttribute = containsAttribute;
        Mandatory = mandatory;
    }
}

public static class OperationFilterContextExtensions
{
    public static CustomAttribute RequireAttribute<T>(this OperationFilterContext context)
    {
        IEnumerable<IFilterMetadata> globalAttributes = context
            .ApiDescription
            .ActionDescriptor
            .FilterDescriptors
            .Select(p => p.Filter);

        object[] controllerAttributes = context
            .MethodInfo?
            .DeclaringType?
            .GetCustomAttributes(true) ?? Array.Empty<object>();

        object[] methodAttributes = context
            .MethodInfo?
            .GetCustomAttributes(true)?? Array.Empty<object>();

        List<T> containsHeaderAttributes = globalAttributes
            .Union(controllerAttributes)
            .Union(methodAttributes)
            .OfType<T>()
            .ToList();
        
        return containsHeaderAttributes.Count == 0 
            ? new CustomAttribute(false, false) 
            : new CustomAttribute(true, false);
    }
}