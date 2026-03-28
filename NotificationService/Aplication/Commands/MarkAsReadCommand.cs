namespace NotificationService.Aplication.Commands
{
    public record MarkAsReadCommand(
        Guid RecipientId,
        IEnumerable<Guid>? NotificationIds  // null = marcar todas
    );
}
