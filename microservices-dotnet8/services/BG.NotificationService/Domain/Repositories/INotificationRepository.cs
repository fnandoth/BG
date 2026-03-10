using BG.NotificationService.Domain.Entities;

namespace BG.NotificationService.Domain.Repositories;

public interface INotificationRepository
{
    IReadOnlyCollection<NotificationAggregate> GetAll();
}
