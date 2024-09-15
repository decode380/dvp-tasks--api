
using System.Reflection;
using Application;
using Infrastructure;
using Microsoft.OpenApi.Models;
using Webapi;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



var info = new OpenApiInfo()
{
    Title = "Dvp tasks documentation",
    Version = "v1",
    Description = "Dvp tasks",
    Contact = new OpenApiContact()
    {
        Name = "Miguel Fandiño",
        Email = "decode380@gmail.com",
    }

};
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", info);

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPresentationLayer();

builder.Services.AddControllers();

builder.Services.AddCors( options => {
    options.AddPolicy(
        name: "corsApp",
        policy  =>{policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();}
    );
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(u =>
        {
            u.RouteTemplate = "swagger/{documentName}/swagger.json";
        });
    app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = "swagger";
            c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Dvp tasks API");
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("corsApp");

app.AddAppMiddlewares();

app.MapControllers();

app.Run();
