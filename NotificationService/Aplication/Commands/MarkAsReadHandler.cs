using NotificationService.Domain.Interfaces;

namespace NotificationService.Aplication.Commands
{
    public class MarkAsReadHandler
    {
        private readonly INotificationRepository _repo;

        public MarkAsReadHandler(INotificationRepository repo) => _repo = repo;

        public Task HandleAsync(MarkAsReadCommand command, CancellationToken ct = default) =>
            _repo.MarkAsReadAsync(command.RecipientId, command.NotificationIds, ct);
    }

}
