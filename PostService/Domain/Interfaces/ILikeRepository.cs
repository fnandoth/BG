using PostService.Aplication.DTOs;

namespace PostService.Domain.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> ToggleLikeAsync(Guid postId, Guid userId);
        Task<bool> IsLikedAsync(Guid postId, Guid userId);
        Task<int> GetLikesCountAsync(Guid postId);
        Task<IEnumerable<TimelinePostDto>> GetLikedPostsByUserAsync(Guid userId, int page, int pageSize);
    }
}