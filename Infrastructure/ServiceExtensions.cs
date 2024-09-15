using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class ServiceExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration config){
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(
            config.GetConnectionString("DvpTasksConnection"),
            b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)
        ));
    }
}