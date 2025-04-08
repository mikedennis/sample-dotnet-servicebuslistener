using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AzSbListener;

public class QueueConnectionHealthCheck : IHealthCheck
{
    private volatile bool isQueueConnected;
    public bool IsQueueConnected
    {
        get => isQueueConnected;
        set => isQueueConnected = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (IsQueueConnected)
        {
            return Task.FromResult(HealthCheckResult.Healthy("The queue connection is healthy"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("The queue connection in unhealthy"));
    }
}