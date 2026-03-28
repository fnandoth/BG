using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationRepository
    {
        // El consumer escribe
        Task AddAsync(Notification notification, CancellationToken ct = default);

        // El usuario lee su lista paginada
        Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetByRecipientAsync(
            Guid recipientId,
            int page,
            int pageSize,
            bool onlyUnread = false,
            CancellationToken ct = default);

        // Cuántas no leídas tiene (para el badge del icono de campana, quizas se cambia a futuro de momento se mantiene)
        Task<int> CountUnreadAsync(Guid recipientId, CancellationToken ct = default);

        // Marca una o todas como leídas
        Task MarkAsReadAsync(Guid recipientId, IEnumerable<Guid>? notificationIds, CancellationToken ct = default);
    }
}
