using MassTransit;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers
{
    public class PostRepostedConsumer : IConsumer<PostRepostedEvent>
    {
        private readonly INotificationRepository _repo;
        public PostRepostedConsumer(INotificationRepository repo) => _repo = repo;

        public async Task Consume(ConsumeContext<PostRepostedEvent> context)
        {
            var e = context.Message;

            // No notificar si alguien repostea su propio post
            if (e.ReposterUserId == e.AuthorId) return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = e.AuthorId,
                Type = NotificationType.Repost,
                ActorSnapshot = new ActorSnapshot(
                    UserId: e.ReposterUserId,
                    Username: e.ReposterUsername,
                    DisplayName: e.ReposterDisplayName,
                    AvatarUrl: e.ReposterAvatarUrl
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