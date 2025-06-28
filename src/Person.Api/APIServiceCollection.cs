using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Cors.Infrastructure;


namespace Person.Api;

public static class APIServiceCollection
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {

        var applicationAssembly = typeof(APIServiceCollection).Assembly;

  
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method}{context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                if (activity != null)
                {
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity.TraceId);
                    context.ProblemDetails.Extensions.TryAdd("spanId", activity.SpanId);
                }

            };
        });
 



        services.AddCorsFromOrigin(configuration);
     
  
        services.AddHealthChecks(configuration);
        return services;
    }


    public static IServiceCollection AddCorsFromOrigin(this IServiceCollection services, IConfiguration configuration)
    {
   
        var origins = configuration.GetSection("CorsOrigins_PermittedClients").Get<string[]>();


        if (origins == null || origins.Length == 0)
        {
            throw new ArgumentException("CorsOrigins_PermittedClients configuration section is missing or empty.");
        }
        services.AddCors(options =>
        {
            options.AddPolicy("Cors_Policy", builder =>
            {
                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(origins)
                    .AllowCredentials();
            });
        });
        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("Person_Health", () => HealthCheckResult.Healthy());
        return services;
    }
 

}




