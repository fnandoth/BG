using Microsoft.EntityFrameworkCore;
using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;

namespace PostService.Infrastructure.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly PostContext _context;

        public PostRepository(PostContext context)
        {
            _context = context;
        }

        // ─── Privado: obtener post por id ─────────────────────────────────────────

        private async Task<Post> GetPostEntityByIdAsync(Guid postId)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted)
                ?? throw new InvalidOperationException($"No se pudo encontrar el post '{postId}'.");
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        public async Task<PostDetailDto> CreateAsync(CreatePostRequest request, Guid authorId, AuthorSnapshot author)
        {
            var post = request.ToEntity(authorId, author);

            _context.Posts.Add(post);
            await SaveChangesAsync();

            return post.ToDetailDto();
        }

        // ─── Delete (soft) ────────────────────────────────────────────────────────

        public async Task<bool> DeleteAsync(Guid postId, Guid requesterId)
        {
            var post = await GetPostEntityByIdAsync(postId);

            if (post.AuthorId != requesterId)
                throw new UnauthorizedAccessException(
                    $"El usuario '{requesterId}' no tiene permiso para eliminar este post.");

            post.IsDeleted = true;
            _context.Posts.Update(post);
            await SaveChangesAsync();

            return true;
        }

        // ─── Get post detallado ───────────────────────────────────────────────────
        // en las siguientes dos funciones se les añade null a  p.ToDetailDto(null) porque no queremos cargar el post citado dentro del detalle del post,
        // para evitar cargas innecesarias y mejorar el rendimiento. Si se quisiera cargar el post citado,
        // se podría hacer una consulta adicional para obtenerlo y luego mapearlo a un PostSummaryDto, pero en este caso se opta por no incluirlo en el detalle del post.

        //TODO: investigar una mejor manera de cargar, quizas modificando el mapeo a DTO para que solo cargue el post citado si es necesario,
        //o utilizando un DTO específico para el detalle del post que incluya el post citado como una propiedad opcional.
        public async Task<PostDetailDto?> GetPostDetailAsync(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.QuotedPost)
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            if (post is null) return null;

            var replies = await _context.Posts
                .Where(p => p.ParentPostId == postId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => p.ToDetailDto(null))
                .ToListAsync();

            return post.ToDetailDto(replies);
        }

        // ─── Get replies ──────────────────────────────────────────────────────────

        public async Task<IEnumerable<PostDetailDto>> GetRepliesAsync(Guid postId, int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.ParentPostId == postId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToDetailDto(null))
                .ToListAsync();
        }

        // ─── Get citas ────────────────────────────────────────────────────────────

        public async Task<IEnumerable<PostSummaryDto>> GetQuotesAsync(Guid postId, int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.QuotedPostId == postId && p.PostType == PostType.Quote && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToSummaryDto())
                .ToListAsync();
        }

        // ─── Get timeline ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<TimelinePostDto>> GetTimelineAsync(Guid userId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.QuotedPost)
                .Where(p => p.AuthorId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await MapToTimelineDtosAsync(posts);
        }

        // ─── Get posts de un usuario ──────────────────────────────────────────────

        public async Task<IEnumerable<TimelinePostDto>> GetUserPostsAsync(Guid userId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.QuotedPost)
                .Where(p => p.AuthorId == userId
                         && p.PostType != PostType.Reply
                         && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await MapToTimelineDtosAsync(posts);
        }

        // ─── Repost ───────────────────────────────────────────────────────────────

        public async Task<TimelinePostDto> RepostAsync(Guid postId, Guid authorId, AuthorSnapshot author)
        {
            var originalPost = await GetPostEntityByIdAsync(postId);

            var alreadyReposted = await _context.Posts.AnyAsync(p =>
                p.AuthorId == authorId &&
                p.OriginalPostId == postId &&
                p.PostType == PostType.Repost &&
                !p.IsDeleted);

            if (alreadyReposted)
                throw new InvalidOperationException("El usuario ya hizo repost de este post.");

            var repost = new CreatePostRequest
            {
                Content = string.Empty,
                PostType = PostType.Repost,
                RepostedPostId = postId
            }.ToEntity(authorId, author);

            _context.Posts.Add(repost);

            originalPost.RepostsCount++;
            _context.Posts.Update(originalPost);

            await SaveChangesAsync();

            return repost.ToTimelineDto(originalPost);
        }

        // ─── Quote ────────────────────────────────────────────────────────────────

        public async Task<PostDetailDto> QuotePostAsync(CreatePostRequest request, Guid authorId, AuthorSnapshot author)
        {
            var quotedPost = await GetPostEntityByIdAsync(
                request.QuotedPostId ?? throw new ArgumentException("QuotedPostId es requerido para una cita."));

            var quote = request.ToEntity(authorId, author);

            _context.Posts.Add(quote);

            quotedPost.QuotesCount++;
            _context.Posts.Update(quotedPost);

            await SaveChangesAsync();

            return quote.ToDetailDto();
        }

        // ─── Privado: mapear reposts a su post original ───────────────────────────

        private async Task<IEnumerable<TimelinePostDto>> MapToTimelineDtosAsync(List<Post> posts)
        {
            var repostOriginalIds = posts
                .Where(p => p.PostType == PostType.Repost && p.OriginalPostId.HasValue)
                .Select(p => p.OriginalPostId!.Value)
                .Distinct()
                .ToList();

            var originalPosts = repostOriginalIds.Any()
                ? await _context.Posts
                    .Where(p => repostOriginalIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id)
                : new Dictionary<Guid, Post>();

            return posts.Select(p =>
            {
                Post? reposted = p.PostType == PostType.Repost && p.OriginalPostId.HasValue
                    ? originalPosts.GetValueOrDefault(p.OriginalPostId.Value)
                    : null;

                return p.ToTimelineDto(reposted);
            }).ToList();
        }

        // ─── SaveChanges ──────────────────────────────────────────────────────────

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}