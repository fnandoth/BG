namespace PostService.Domain.ValueObjects
{
    public record AuthorSnapshot(
        Guid   UserId,
        string Username,
        string DisplayName,
        string? AvatarUrl,
        bool   IsVerified
    )
    {
        public AuthorSnapshot() : this(default, "", "", null, false) { }
    }
}
