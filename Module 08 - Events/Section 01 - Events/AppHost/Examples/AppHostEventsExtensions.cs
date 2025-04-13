using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppHost.Examples;

public static class AppHostEventsExtensions
{
    public static void SubscribeToAppHostEvents(this IDistributedApplicationBuilder builder)
    {
        builder.Eventing.Subscribe<BeforeStartEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** BeforeStartEvent");
                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** AfterEndpointsAllocatedEvent");
                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** AfterResourcesCreatedEvent");
                return Task.CompletedTask;
            });
    }
}
