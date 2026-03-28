namespace Shared.Contracts.Events
{
    public record PostLikedEvent(
        Guid PostId,
        Guid AuthorId,
        Guid LikerUserId,
        string LikerUsername,
        string LikerDisplayName,
        string? LikerAvatarUrl,
        string ContentPreview,
        DateTimeOffset OccurredAt
    );

}
