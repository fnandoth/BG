using BG.NotificationService.Domain.Repositories;
using BG.NotificationService.Domain.Entities;

namespace BG.NotificationService.Infrastructure.Persistence;

public sealed class InMemoryNotificationRepository : INotificationRepository
{
    private static readonly IReadOnlyCollection<NotificationAggregate> Seed =
    [
        new() { Name = "Notification root" }
    ];

    public IReadOnlyCollection<NotificationAggregate> GetAll() => Seed;
}
