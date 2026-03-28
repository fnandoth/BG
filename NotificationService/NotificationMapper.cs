using NotificationService.Aplication.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService
{
    public static class NotificationMapper
    {
        public static NotificationDto ToDto(this Notification n) => new(
            Id: n.Id,
            Type: n.Type.ToString().ToLowerInvariant(),
            IsRead: n.IsRead,
            CreatedAt: n.CreatedAt,
            Actor: new ActorDto(
                UserId: n.ActorSnapshot.UserId,
                Username: n.ActorSnapshot.Username,
                DisplayName: n.ActorSnapshot.DisplayName,
                AvatarUrl: n.ActorSnapshot.AvatarUrl
            ),
            Post: n.EntitySnapshot is null ? null : new PostPreviewDto(
                PostId: n.EntitySnapshot.PostId,
                ContentPreview: n.EntitySnapshot.ContentPreview
            )
        );
    }

}
