using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppHost.Examples;

public static class ResourceEventsExtensions
{
    public static void SubscribeToResourceEvents(this IDistributedApplicationBuilder builder, ProjectResource api)
    {
        builder.Eventing.Subscribe<BeforeResourceStartedEvent>(api,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** BeforeResourceStartedEvent for {ResourceName}", @event.Resource.Name);
                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<ResourceReadyEvent>(api,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** ResourceReadyEvent for {ResourceName}", @event.Resource.Name);
                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(api,
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** ConnectionStringAvailableEvent for {ResourceName}", @event.Resource.Name);
                return Task.CompletedTask;
            });
    }
}
