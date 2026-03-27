using PostService.Domain.ValueObjects;

namespace PostService.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public AuthorSnapshot AuthorSnapshot { get; set; } = default!;  // de tipo JSONB
        public string Content { get; set; } = default!;
        public string[]? MediaUrls { get; set; }
        public PostType PostType { get; set; }
        public Guid? ParentPostId { get; set; }
        public Guid? QuotedPostId { get; set; }
        public Guid? OriginalPostId { get; set; }
        public int LikesCount { get; set; }
        public int RepostsCount { get; set; }
        public int RepliesCount { get; set; }
        public int QuotesCount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // navegación 
        public Post? ParentPost { get; set; }
        public Post? QuotedPost { get; set; }
    }

    public enum PostType { Post, Reply, Repost, Quote }

}
