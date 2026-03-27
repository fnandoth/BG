using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;

namespace PostService
{
    public static class PostMapper
    {
        // Post -> PostDetailDto
        public static PostDetailDto ToDetailDto(this Post post, List<PostDetailDto>? replies = null) => new()
        {
            Id = post.Id,
            Content = post.Content,
            PostType = post.PostType,
            Author = post.AuthorSnapshot,
            LikesCount = post.LikesCount,
            RepliesCount = post.RepliesCount,
            RepostsCount = post.RepostsCount,
            QuotesCount = post.QuotesCount,
            QuotedPost = post.QuotedPost?.ToSummaryDto(),
            Replies = replies ?? new List<PostDetailDto>(),
            CreatedAt = post.CreatedAt
        };

        // Post -> TimelinePostDto
        public static TimelinePostDto ToTimelineDto(this Post post, Post? repostedPost = null) => new()
        {
            Id = post.Id,
            Content = post.Content,
            PostType = post.PostType,
            Author = post.AuthorSnapshot,
            LikesCount = post.LikesCount,
            RepliesCount = post.RepliesCount,
            RepostsCount = post.RepostsCount,
            QuotesCount = post.QuotesCount,
            QuotedPost = post.QuotedPost?.ToSummaryDto(),
            RepostedPost = repostedPost?.ToSummaryDto(),
            CreatedAt = post.CreatedAt
        };

        // Post -> PostSummaryDto
        public static PostSummaryDto ToSummaryDto(this Post post) => new()
        {
            Id = post.Id,
            Content = post.Content,
            Author = post.AuthorSnapshot
        };

        // CreatePostRequest -> Post
        public static Post ToEntity(this CreatePostRequest request, Guid authorId, AuthorSnapshot authorSnapshot) => new()
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            AuthorSnapshot = authorSnapshot,
            Content = request.Content,
            MediaUrls = request.MediaUrls,
            PostType = request.PostType,
            ParentPostId = request.ParentPostId,
            QuotedPostId = request.QuotedPostId,
            OriginalPostId = request.RepostedPostId,
            LikesCount = 0,
            RepostsCount = 0,
            RepliesCount = 0,
            QuotesCount = 0,
            IsDeleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

}
