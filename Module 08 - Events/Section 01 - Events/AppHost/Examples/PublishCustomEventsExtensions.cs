using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppHost.Examples;

public static class PublishCustomEventsExtensions
{
    public static void PublishCustomEvents(
        this IDistributedApplicationBuilder builder,
        IResource resource)
    {
        // Subscribe to our custom events...

        builder.Eventing.Subscribe<DemoAppEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("*** DemoAppEvent");
                return Task.CompletedTask;
            });

        builder.Eventing.Subscribe<DemoResourceEvent>(
            static (@event, cancellationToken) =>
            {
                var logger = @event.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"*** DemoResourceEvent for {@event.Resource.Name}");
                return Task.CompletedTask;
            });

        // Publish them...

        var serviceProvider = builder.Services.BuildServiceProvider();

        builder.Eventing.PublishAsync(new DemoAppEvent(serviceProvider));
        builder.Eventing.PublishAsync(new DemoResourceEvent(serviceProvider, resource));

        // Example with explicit choice of behaviour (default is BlockingSequential)
        // builder.Eventing.PublishAsync(new DemoAppEvent(serviceProvider),
        //     EventDispatchBehavior.BlockingSequential);
    }
}

internal class DemoAppEvent(IServiceProvider serviceProvider) : IDistributedApplicationEvent
{
    public IServiceProvider Services { get; set; } = serviceProvider;
}

internal class DemoResourceEvent(
    IServiceProvider serviceProvider,
    IResource resource) : IDistributedApplicationResourceEvent
{
    public IResource Resource { get; } = resource;
    public IServiceProvider Services { get; set; } = serviceProvider;
}
