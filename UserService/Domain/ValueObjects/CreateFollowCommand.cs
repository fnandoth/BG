namespace UserService.Domain.ValueObjects
{
    public record CreateFollowCommand
    (
        Guid FollowedUserId,     // recipient
        Guid FollowerUserId,
        string FollowerUsername,
        string FollowerDisplayName,
        string? FollowerAvatarUrl
    );
}
