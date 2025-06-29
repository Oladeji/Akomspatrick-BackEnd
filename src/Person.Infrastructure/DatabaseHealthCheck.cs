using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Person.Infrastructure
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly PersonDbContext _personDbContext;
        public DatabaseHealthCheck(PersonDbContext personDbContext)
        {
            _personDbContext = personDbContext ?? throw new ArgumentNullException(nameof(personDbContext));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            bool isHealthy;
            string? error = null;

            try
            {
                isHealthy = await _personDbContext.Database.CanConnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                isHealthy = false;
                error = ex.Message;
            }
            stopwatch.Stop();

            var provider = _personDbContext.Database.ProviderName ?? "Unknown";
            var data = new Dictionary<string, object>
            {
                { "Provider", provider },
                { "ElapsedMilliseconds", stopwatch.ElapsedMilliseconds },
                { "Database", _personDbContext.Database.GetDbConnection().Database },
                { "DataSource", _personDbContext.Database.GetDbConnection().DataSource }
            };

            if (!string.IsNullOrEmpty(error))
            {
                data.Add("Error", error);
            }

            return isHealthy
       ? HealthCheckResult.Healthy($"Database connection successful (Provider: {provider})", data)
       : HealthCheckResult.Unhealthy(
           $"Database connection failed (Provider: {provider}). Error: {error}",
           null,
           data
       );

        }
    }
}
