
using System.Reflection;
using System.Text;
using Application.Behaviours;
using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public static class ServiceExtensions{
    public static void AddApplicationLayer(this IServiceCollection services, IConfiguration config){
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        var jwtKey = config.GetValue<string>("JwtSettings:SecretKey") ?? throw new ArgumentNullException("Jwt secret key no provided");
        var jwtAudience = config.GetValue<string>("JwtSettings:Audience") ?? throw new ArgumentNullException("Jwt Audience not provided");
        var jwtIssuer = config.GetValue<string>("JwtSettings:Issuer") ?? throw new ArgumentNullException("Jwt Issuer not provided");

        services.Configure<JwtSettings>(options => {
            options.SecretKey = jwtKey;
            options.Issuer = jwtIssuer;
            options.Audience = jwtAudience;
        });
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IRoleFeatureService, RoleFeatureService>();

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer( options => {
                options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
    }
}