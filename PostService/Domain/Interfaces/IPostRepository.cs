using PostService.Aplication.DTOs;
using PostService.Domain.ValueObjects;

namespace PostService.Domain.Interfaces
{
    public interface IPostRepository
    {
        // ─── Escritura ────────────────────────────────────────────────────────────
        Task<PostDetailDto> CreateAsync(CreatePostRequest request, Guid authorId, AuthorSnapshot author);
        Task<TimelinePostDto> RepostAsync(Guid postId, Guid authorId, AuthorSnapshot author);
        Task<PostDetailDto> QuotePostAsync(CreatePostRequest request, Guid authorId, AuthorSnapshot author);
        Task<bool> DeleteAsync(Guid postId, Guid requesterId);

        // ─── Lectura ──────────────────────────────────────────────────────────────
        Task<IEnumerable<TimelinePostDto>> GetTimelineAsync(Guid userId, int page, int pageSize);
        Task<PostDetailDto?> GetPostDetailAsync(Guid postId);
        Task<IEnumerable<PostDetailDto>> GetRepliesAsync(Guid postId, int page, int pageSize);
        Task<IEnumerable<PostSummaryDto>> GetQuotesAsync(Guid postId, int page, int pageSize);
        Task<IEnumerable<TimelinePostDto>> GetUserPostsAsync(Guid userId, int page, int pageSize);
    }
}