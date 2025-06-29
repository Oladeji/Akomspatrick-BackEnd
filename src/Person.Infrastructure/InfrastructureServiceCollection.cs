using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Person.Infrastructure
{
    public static class InfrastructureServiceCollection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            var applicationAssembly = typeof(InfrastructureServiceCollection).Assembly;

                services.AddDbContext<PersonDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); 



            services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("Database");
            return services;
        }


    }
}
