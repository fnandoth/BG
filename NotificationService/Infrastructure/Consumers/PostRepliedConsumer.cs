using MassTransit;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers
{
    public class PostRepliedConsumer : IConsumer<PostRepliedEvent>
    {
        private readonly INotificationRepository _repo;
        public PostRepliedConsumer(INotificationRepository repo) => _repo = repo;

        public async Task Consume(ConsumeContext<PostRepliedEvent> context)
        {
            var e = context.Message;

            // No notificar si alguien responde su propio post
            if (e.ReplierUserId == e.AuthorId) return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = e.AuthorId,
                Type = NotificationType.Reply,
                ActorSnapshot = new ActorSnapshot(
                    UserId: e.ReplierUserId,
                    Username: e.ReplierUsername,
                    DisplayName: e.ReplierDisplayName,
                    AvatarUrl: e.ReplierAvatarUrl
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