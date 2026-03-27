using Microsoft.AspNetCore.Mvc;
using PostService.Aplication.DTOs;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;

        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreatePostRequest request,
            [FromHeader(Name = "X-User-Id")] Guid authorId,
            [FromHeader(Name = "X-Author-Snapshot")] string snapshotJson)
        {
            var author = System.Text.Json.JsonSerializer.Deserialize<AuthorSnapshot>(snapshotJson)
                ?? throw new ArgumentException("AuthorSnapshot inválido.");

            var post = await _postRepository.CreateAsync(request, authorId, author);
            return CreatedAtAction(nameof(GetPostDetailAsync), new { postId = post.Id }, post);
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> DeleteAsync(
            Guid postId,
            [FromHeader(Name = "X-User-Id")] Guid requesterId)
        {
            await _postRepository.DeleteAsync(postId, requesterId);
            return NoContent();
        }

        // ─── Get post detallado ───────────────────────────────────────────────────

        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetPostDetailAsync(Guid postId)
        {
            var post = await _postRepository.GetPostDetailAsync(postId);
            if (post is null) return NotFound();
            return Ok(post);
        }

        // ─── Get replies ──────────────────────────────────────────────────────────

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
        public async Task<IActionResult> RepostAsync(
            Guid postId,
            [FromHeader(Name = "X-User-Id")] Guid authorId,
            [FromHeader(Name = "X-Author-Snapshot")] string snapshotJson)
        {
            var author = System.Text.Json.JsonSerializer.Deserialize<AuthorSnapshot>(snapshotJson)
                ?? throw new ArgumentException("AuthorSnapshot inválido.");

            var repost = await _postRepository.RepostAsync(postId, authorId, author);
            return Ok(repost);
        }

        // ─── Quote ────────────────────────────────────────────────────────────────

        [HttpPost("{postId:guid}/quote")]
        public async Task<IActionResult> QuotePostAsync(
            Guid postId,
            [FromBody] CreatePostRequest request,
            [FromHeader(Name = "X-User-Id")] Guid authorId,
            [FromHeader(Name = "X-Author-Snapshot")] string snapshotJson)
        {
            var author = System.Text.Json.JsonSerializer.Deserialize<AuthorSnapshot>(snapshotJson)
                ?? throw new ArgumentException("AuthorSnapshot inválido.");

            request.QuotedPostId = postId;
            request.PostType = PostService.Domain.Entities.PostType.Quote;

            var quote = await _postRepository.QuotePostAsync(request, authorId, author);
            return CreatedAtAction(nameof(GetPostDetailAsync), new { postId = quote.Id }, quote);
        }
    }
}