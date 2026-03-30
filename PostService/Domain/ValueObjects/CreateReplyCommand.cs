namespace PostService.Domain.ValueObjects
{
    public record CreateReplyCommand(
        Guid ParentPostId,       // el post al que se responde → de aquí sacas al AuthorId
        Guid ReplyPostId,        // el post nuevo ya guardado en DB
        Guid ReplierUserId,
        string ReplierUsername,
        string ReplierDisplayName,
        string? ReplierAvatarUrl
    );
}
