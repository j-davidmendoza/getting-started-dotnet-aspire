using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api;

public class MyHealthCheck : IHealthCheck
{
    public static bool Healthy { get; set; } = true;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        context.Registration.Tags.Add("live");
        return Task.FromResult(Healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}