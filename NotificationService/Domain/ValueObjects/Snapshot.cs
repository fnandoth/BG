namespace NotificationService.Domain.ValueObjects
{
    public record ActorSnapshot(Guid UserId, string Username, string DisplayName, string? AvatarUrl)
    {
        public ActorSnapshot() : this(default, "", "", null) { }
    }

    public record EntitySnapshot(Guid PostId, string ContentPreview)
    {
        public EntitySnapshot() : this(default, "") { }
    }
}