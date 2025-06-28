
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Person.Shared.CQRS;
namespace Person.Application;
public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(ApplicationServiceCollection).Assembly;
        services.AddMediater(applicationAssembly);
        return services;
    }
}
