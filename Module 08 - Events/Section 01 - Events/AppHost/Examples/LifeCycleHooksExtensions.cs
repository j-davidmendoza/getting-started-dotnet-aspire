using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace AppHost.Examples;

/// <summary>
/// Microsoft recommend that you use events, rather than lifecycle hooks.
/// This code is just included for completeness.
/// </summary>
public static class LifeCycleHooksExtensions
{
    public static void SubscribeToLifeCycleHooks(this IDistributedApplicationBuilder builder)
    {
        builder.Services.AddLifecycleHook<LifeCycleHook>();
    }
}

internal class LifeCycleHook(ILogger<LifeCycleHook> logger) : IDistributedApplicationLifecycleHook
{
    public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("*** LifeCycleHook::BeforeStartAsync");
        return Task.CompletedTask;
    }
}
