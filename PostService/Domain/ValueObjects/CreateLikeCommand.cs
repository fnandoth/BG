namespace PostService.Domain.ValueObjects
{
    public record CreateLikeCommand(
        Guid UserId,
        Guid PostId,
        string Username,
        string DisplayName,
        string AvatarUrl
    );
}
