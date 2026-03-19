using SharedKernel.BG.SharedKernel.Events;
using SharedKernel.BG.SharedKernel.Interfaces;

namespace BG.IdentityService.Infrastructure.Events;

public sealed class InMemoryEventBus(ILogger<InMemoryEventBus> logger) : IEventBus
{
    public Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default) where T : IntegrationEvent
    {
        logger.LogInformation("Publishing integration event {EventType}: {@Event}", integrationEvent.EventType, integrationEvent);
        return Task.CompletedTask;
    }
}