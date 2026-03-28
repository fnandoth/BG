namespace NotificationService.Domain.ValueObjects
{
    public record ActorSnapshot(Guid UserId, string Username, string DisplayName, string? AvatarUrl);
    public record EntitySnapshot(Guid PostId, string ContentPreview);
}
