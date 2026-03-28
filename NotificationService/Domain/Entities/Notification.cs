using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public ActorSnapshot ActorSnapshot { get; set; } = default!;
        public EntitySnapshot? EntitySnapshot { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public enum NotificationType { Like, Repost, Reply, Quote, Mention, Follow, Dm }
}
