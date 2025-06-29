using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.Extensions.DependencyInjection;


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
        services.AddInstrumentation(configuration);
        services.AddHealthChecks(configuration);
        services.AddHealthChecksWithUI();
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


    public static IServiceCollection AddHealthChecksWithUI(this IServiceCollection services)
    {
       services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(15);
            options.MaximumHistoryEntriesPerEndpoint(60);
            options.AddHealthCheckEndpoint("Person API", "/health");
        })
    .AddInMemoryStorage();
        return services;
    }




private static IServiceCollection AddInstrumentation(this IServiceCollection services, IConfiguration configuration)
    {

        string serviceName = configuration["Otlp:ServiceName"] ?? Assembly.GetExecutingAssembly().GetName().Name;
        string version = configuration["Otlp:Version"];
        var otlpEndpoint = new Uri(configuration["Otlp:Endpoint"] );

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                .AddService(serviceName)
                .AddAttributes(new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("service.version", Assembly.GetExecutingAssembly().GetName().Version!.ToString()),

                });
            })

            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource(serviceName + ".Tracing")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                     .AddSqlClientInstrumentation() 
      
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = otlpEndpoint;

                    });

            })
            .WithMetrics(meterProviderBuilder =>
            {
                meterProviderBuilder
               .AddMeter(
                          "Microsoft.AspNetCore.Hosting",
                          "Microsoft.AspNetCore.Server.Kestrel",
                          "System.Net.Http",
                          "System.Runtime",
                          "System.Threading.Tasks")

                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                     .AddProcessInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = otlpEndpoint;

                    });
            });


        return services;


    }



}




