using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Person.Infrastructure
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly PersonDbContext _personDbContext;
        public DatabaseHealthCheck(PersonDbContext personDbContext)
        {
            _personDbContext = personDbContext ?? throw new ArgumentNullException(nameof(personDbContext));
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = _personDbContext.Database.CanConnect();
            return Task.FromResult(isHealthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
        }
    }
}
