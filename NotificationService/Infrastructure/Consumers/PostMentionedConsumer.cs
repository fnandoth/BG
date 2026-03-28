using MassTransit;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers
{
    public class PostMentionedConsumer : IConsumer<PostMentionedEvent>
    {
        private readonly INotificationRepository _repo;

        public PostMentionedConsumer(INotificationRepository repo) => _repo = repo;

        public async Task Consume(ConsumeContext<PostMentionedEvent> context)
        {
            var e = context.Message;

            // No notificar si alguien se menciona a sí mismo
            if (e.MentionerUserId == e.MentionedUserId) return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = e.MentionedUserId,
                Type = NotificationType.Mention,
                ActorSnapshot = new ActorSnapshot(
                    UserId: e.MentionerUserId,
                    Username: e.MentionerUsername,
                    DisplayName: e.MentionerDisplayName,
                    AvatarUrl: e.MentionerAvatarUrl
                ),
                EntitySnapshot = new EntitySnapshot(
                    PostId: e.PostId,
                    ContentPreview: e.ContentPreview
                ),
                IsRead = false,
                CreatedAt = e.OccurredAt
            };

            await _repo.AddAsync(notification, context.CancellationToken);
        }
    }
}