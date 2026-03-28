using MassTransit;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace NotificationService.Infrastructure.Consumers
{
    public class UserFollowedConsumer : IConsumer<UserFollowedEvent>
    {
        private readonly INotificationRepository _repo;
        public UserFollowedConsumer(INotificationRepository repo) => _repo = repo;

        public async Task Consume(ConsumeContext<UserFollowedEvent> context)
        {
            var e = context.Message;
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = e.FollowedUserId,
                Type = NotificationType.Follow,
                ActorSnapshot = new ActorSnapshot(
                    UserId: e.FollowerUserId,
                    Username: e.FollowerUsername,
                    DisplayName: e.FollowerDisplayName,
                    AvatarUrl: e.FollowerAvatarUrl
                ),
                EntitySnapshot = null,   // ← los follows no tienen post asociado
                IsRead = false,
                CreatedAt = e.OccurredAt
            };
            await _repo.AddAsync(notification, context.CancellationToken);
        }
    }

}
