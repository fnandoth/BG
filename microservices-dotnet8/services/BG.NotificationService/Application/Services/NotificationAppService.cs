using BG.NotificationService.Domain.Repositories;
using BG.NotificationService.Application.Abstractions;

namespace BG.NotificationService.Application.Services;

public sealed class NotificationAppService(INotificationRepository repository) : INotificationAppService
{
    public object GetBlueprint() => new
    {
        boundedContext = "Notification",
        purpose = "notifications/read-status",
        entities = repository.GetAll().Select(x => x.Name)
    };
}
