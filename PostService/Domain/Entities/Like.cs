namespace PostService.Domain.Entities
{
    public class Like
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // navegacion 
        public Post Post { get; set; } = default!;
    }
}
