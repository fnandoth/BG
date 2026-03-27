using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;

namespace PostService.Aplication.DTOs
{
    public class TimelinePostDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public PostType PostType { get; set; }

        public AuthorSnapshot Author { get; set; } = default!;

        public int LikesCount { get; set; }
        public int RepliesCount { get; set; }
        public int RepostsCount { get; set; }
        public int QuotesCount { get; set; }

        public PostSummaryDto? QuotedPost { get; set; }
        public PostSummaryDto? RepostedPost { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
