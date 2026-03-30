using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Aplication.Commands;
using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/posts")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly CreateReplyHandler _createReplyHandler;
        private readonly CreateRepostHandler _createRepostHandler;

        public PostController(
            IPostRepository postRepository,
            CreateReplyHandler createReplyHandler,
            CreateRepostHandler createRepostHandler)
        {
            _postRepository = postRepository;
            _createReplyHandler = createReplyHandler;
            _createRepostHandler = createRepostHandler;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido: NameIdentifier no encontrado."));

        private string CurrentUsername => User.FindFirstValue(ClaimTypes.Name)
            ?? throw new UnauthorizedAccessException("Token inválido: Name no encontrado.");

        private string CurrentDisplayName => User.FindFirstValue("display_name")
            ?? throw new UnauthorizedAccessException("Token inválido: display_name no encontrado.");

        private string CurrentAvatar => User.FindFirstValue("avatar_url") ?? string.Empty;

        private AuthorSnapshot CurrentAuthorSnapshot => new(
            CurrentUserId,
            CurrentUsername,
            CurrentDisplayName,
            CurrentAvatar,
            false);
        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePostRequest request, CancellationToken ct)
        {
            var post = await _postRepository.CreateAsync(request, CurrentUserId, CurrentAuthorSnapshot);

            if (post.PostType == PostType.Reply && request.ParentPostId.HasValue)
            {
                await _createReplyHandler.HandleAsync(new CreateReplyCommand(
                    ParentPostId: request.ParentPostId.Value,
                    ReplyPostId: post.Id,
                    ReplierUserId: CurrentUserId,
                    ReplierUsername: CurrentUsername,
                    ReplierDisplayName: CurrentDisplayName,
                    ReplierAvatarUrl: CurrentAvatar), ct);
            }

            return CreatedAtAction(nameof(GetPostDetailAsync), new { postId = post.Id }, post);
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid postId)
        {
            await _postRepository.DeleteAsync(postId, CurrentUserId);
            return NoContent();
        }

        // ─── Get post detallado ───────────────────────────────────────────────────

        [AllowAnonymous]
        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetPostDetailAsync(Guid postId)
        {
            var post = await _postRepository.GetPostDetailAsync(postId);
            if (post is null) return NotFound();
            return Ok(post);
        }

        // ─── Get replies ──────────────────────────────────────────────────────────

        [AllowAnonymous]
        [HttpGet("{postId:guid}/replies")]
        public async Task<IActionResult> GetRepliesAsync(
            Guid postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var replies = await _postRepository.GetRepliesAsync(postId, page, pageSize);
            return Ok(replies);
        }

        // ─── Get citas ────────────────────────────────────────────────────────────

        [AllowAnonymous]
        [HttpGet("{postId:guid}/quotes")]
        public async Task<IActionResult> GetQuotesAsync(
            Guid postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var quotes = await _postRepository.GetQuotesAsync(postId, page, pageSize);
            return Ok(quotes);
        }

        // ─── Get timeline ─────────────────────────────────────────────────────────

        [HttpGet("timeline/{userId:guid}")]
        public async Task<IActionResult> GetTimelineAsync(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var timeline = await _postRepository.GetTimelineAsync(userId, page, pageSize);
            return Ok(timeline);
        }

        // ─── Get posts de un usuario ──────────────────────────────────────────────

        [AllowAnonymous]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserPostsAsync(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var posts = await _postRepository.GetUserPostsAsync(userId, page, pageSize);
            return Ok(posts);
        }

        // ─── Repost ───────────────────────────────────────────────────────────────

        [HttpPost("{postId:guid}/repost")]
        public async Task<IActionResult> RepostAsync(Guid postId, CancellationToken ct)
        {
            var repost = await _postRepository.RepostAsync(postId, CurrentUserId, CurrentAuthorSnapshot);

            await _createRepostHandler.HandleAsync(new CreateRepostCommand(
                RePostId: postId,
                AuthorId: repost.Author.UserId,
                ReposterUserId: CurrentUserId,
                ReposterUsername: CurrentUsername,
                ReposterDisplayName: CurrentDisplayName,
                ReposterAvatarUrl: CurrentAvatar), ct);

            return Ok(repost);
        }

        // ─── Quote ────────────────────────────────────────────────────────────────

        [HttpPost("{postId:guid}/quote")]
        public async Task<IActionResult> QuotePostAsync(
            Guid postId,
            [FromBody] CreatePostRequest request)
        {
            request.QuotedPostId = postId;
            request.PostType = PostType.Quote;

            var quote = await _postRepository.QuotePostAsync(request, CurrentUserId, CurrentAuthorSnapshot);
            return CreatedAtAction(nameof(GetPostDetailAsync), new { postId = quote.Id }, quote);
        }
    }
}