using HealthChecks.UI.Client;
using Person.Api;
using Person.Application;
using Person.Infrastructure;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    {

        var otlpEndpoint = builder.Configuration["Otlp:Endpoint"] ;
        loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otlpEndpoint; // OpenTelemetry Collector endpoint
                options.Protocol = OtlpProtocol.Grpc;
                options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    { "service.name", builder.Configuration["Otlp:ServiceName"] ??Assembly.GetExecutingAssembly().GetName().Name },
                    { "service.version", builder.Configuration["Otlp:Version"] ??"0.0.0" },
                    { "host.name", Environment.MachineName },
                    { "environment", builder.Environment.EnvironmentName },

                };
            });
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddAPIServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices(builder.Configuration);
// Required for UI to work

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Cors_Policy");
app.RegisterEndpoints();
app.UseHttpsRedirection();
//// Use the built-in UI response writer for /health
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Map the HealthChecks UI dashboard
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui"; // UI endpoint
    options.ApiPath = "/health-ui-api"; // API endpoint for the UI
});

//if (app.Environment.IsDevelopment())
//{
//   await app.SeedPersonTestingDataAsync();

//}
//else
//{
//    await app.SeedOnlyPersonTypesDataAsync();
//}
app.MapControllers();

app.Run();

