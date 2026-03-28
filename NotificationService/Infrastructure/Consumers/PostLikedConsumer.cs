using MassTransit;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers
{
    public class PostLikedConsumer : IConsumer<PostLikedEvent>
    {
        private readonly INotificationRepository _repo;

        public PostLikedConsumer(INotificationRepository repo) => _repo = repo;

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var e = context.Message;

            // No realiza la notificacion si el autor del post es el mismo que le dio like
            if (e.AuthorId == e.LikerUserId) return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = e.AuthorId,
                Type = NotificationType.Like,
                ActorSnapshot = new ActorSnapshot(
                    UserId: e.LikerUserId,
                    Username: e.LikerUsername,
                    DisplayName: e.LikerDisplayName,
                    AvatarUrl: e.LikerAvatarUrl
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
