using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Aplication.Commands;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/likes")]
    [Authorize]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;
        private readonly CreateLikeHandler _createLikeHandler;

        public LikeController(ILikeRepository likeRepository, CreateLikeHandler createLikeHandler)
        {
            _likeRepository = likeRepository;
            _createLikeHandler = createLikeHandler;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido: NameIdentifier no encontrado."));

        private string CurrentUsername => User.FindFirstValue(ClaimTypes.Name)
            ?? throw new UnauthorizedAccessException("Token inválido: Name no encontrado.");

        private string CurrentDisplayName => User.FindFirstValue("display_name")
            ?? throw new UnauthorizedAccessException("Token inválido: display_name no encontrado.");

        private string CurrentAvatar => User.FindFirstValue("avatar_url") ?? string.Empty;

        // ─── Toggle like ──────────────────────────────────────────────────────────

        [HttpPost("{postId:guid}")]
        public async Task<IActionResult> ToggleLikeAsync(Guid postId, CancellationToken ct)
        {
            var liked = await _likeRepository.ToggleLikeAsync(postId, CurrentUserId);

            if (liked)
            {
                await _createLikeHandler.HandleAsync(new CreateLikeCommand
                {
                    PostId = postId,
                    UserId = CurrentUserId,
                    Username = CurrentUsername,
                    DisplayName = CurrentDisplayName,
                    AvatarUrl = CurrentAvatar
                }, ct);
            }

            return Ok(new { liked });
        }

        [AllowAnonymous]
        [HttpGet("{postId:guid}/status/{userId:guid}")]
        public async Task<IActionResult> IsLikedAsync(Guid postId, Guid userId)
        {
            var isLiked = await _likeRepository.IsLikedAsync(postId, userId);
            return Ok(new { isLiked });
        }

        [AllowAnonymous]
        [HttpGet("{postId:guid}/count")]
        public async Task<IActionResult> GetLikesCountAsync(Guid postId)
        {
            var count = await _likeRepository.GetLikesCountAsync(postId);
            return Ok(new { count });
        }

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