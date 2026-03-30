namespace PostService.Domain.ValueObjects
{
    public record CreateMentionedCommand(
        Guid MentionPostId,
        Guid MentionedUserId, 
        Guid MentionerUserId,
        string MentionerUsername,
        string MentionerDisplayName,
        string? MentionerAvatarUrl
    );
}
