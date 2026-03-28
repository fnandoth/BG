namespace PostService.Domain.ValueObjects
{
    public class CreateLikeCommand
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
        public required string AvatarUrl { get; set; }
    }
}
