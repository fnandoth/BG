using Microsoft.AspNetCore.Mvc;
using PostService.Aplication.DTOs;
using PostService.Domain.Interfaces;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/likes")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;

        public LikeController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        // ─── Toggle like ──────────────────────────────────────────────────────────

        [HttpPost("{postId:guid}")]
        public async Task<IActionResult> ToggleLikeAsync(
            Guid postId,
            [FromHeader(Name = "X-User-Id")] Guid userId)
        {
            var liked = await _likeRepository.ToggleLikeAsync(postId, userId);
            return Ok(new { liked });
        }

        // ─── IsLiked ──────────────────────────────────────────────────────────────

        [HttpGet("{postId:guid}/status/{userId:guid}")]
        public async Task<IActionResult> IsLikedAsync(Guid postId, Guid userId)
        {
            var isLiked = await _likeRepository.IsLikedAsync(postId, userId);
            return Ok(new { isLiked });
        }

        // ─── GetLikesCount ────────────────────────────────────────────────────────

        [HttpGet("{postId:guid}/count")]
        public async Task<IActionResult> GetLikesCountAsync(Guid postId)
        {
            var count = await _likeRepository.GetLikesCountAsync(postId);
            return Ok(new { count });
        }

        // ─── GetLikedPostsByUser ──────────────────────────────────────────────────

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetLikedPostsByUserAsync(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var posts = await _likeRepository.GetLikedPostsByUserAsync(userId, page, pageSize);
            return Ok(posts);
        }
    }
}