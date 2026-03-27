using Microsoft.EntityFrameworkCore;
using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.Domain.Interfaces;

namespace PostService.Infrastructure.Data.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly PostContext _context;

        public LikeRepository(PostContext context)
        {
            _context = context;
        }

        // ─── ToggleLike ───────────────────────────────────────────────────────────────

        public async Task<bool> ToggleLikeAsync(Guid postId, Guid userId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted)
                ?? throw new InvalidOperationException($"No se pudo encontrar el post '{postId}'.");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike is not null)
            {
                _context.Likes.Remove(existingLike);
                post.LikesCount = Math.Max(0, post.LikesCount - 1);
                _context.Posts.Update(post);
                await SaveChangesAsync();
                return false; // false = quitó el like
            }

            var like = new Like
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Likes.Add(like);
            post.LikesCount++;
            _context.Posts.Update(post);
            await SaveChangesAsync();

            return true; // true = dio like
        }

        // ─── IsLiked ─── No recuerdo el proposito del pq programe esta parte, quizas para el frontend (de momento se queda) ────────────────────────────

        public async Task<bool> IsLikedAsync(Guid postId, Guid userId)
        {
            return await _context.Likes
                .AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }

        // ─── GetLikesCount ────────────────────────────────────────────────────────

        public async Task<int> GetLikesCountAsync(Guid postId)
        {
            return await _context.Likes
                .CountAsync(l => l.PostId == postId);
        }

        // ─── GetLikedPostsByUser ──────────────────────────────────────────────────

        public async Task<IEnumerable<TimelinePostDto>> GetLikedPostsByUserAsync(Guid userId, int page, int pageSize)
        {
            var posts = await _context.Likes
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Join(
                    _context.Posts.Where(p => !p.IsDeleted).Include(p => p.QuotedPost),
                    like => like.PostId,
                    post => post.Id,
                    (like, post) => post)
                .ToListAsync();

            return posts.Select(p => p.ToTimelineDto()).ToList();
        }

        // ─── SaveChanges ──────────────────────────────────────────────────────────

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}