using PostService.Domain.Entities;

namespace PostService.Aplication.DTOs
{
    public class CreatePostRequest
    {
        public string Content { get; set; } = default!;
        public string[]? MediaUrls { get; set; }

        public PostType PostType { get; set; }

        public Guid? ParentPostId { get; set; }
        public Guid? QuotedPostId { get; set; }
        public Guid? RepostedPostId { get; set; }
    }
}
