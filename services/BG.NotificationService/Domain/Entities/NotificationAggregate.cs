namespace BG.NotificationService.Domain.Entities;

public sealed class NotificationAggregate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = "Notification aggregate";
}
