using MassTransit;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace PostService.Aplication.Commands
{
    public class CreateRepostHandler
    {
        private readonly IPublishEndpoint _bus;
        private readonly IPostRepository _post;

        public CreateRepostHandler(IPublishEndpoint bus,  IPostRepository post)
        {
            _bus = bus;
            _post = post;
        }

        public async Task HandleAsync(CreateRepostCommand cmd, CancellationToken ct)
        {
            var repostPost = await _post.GetPostDetailAsync(cmd.RePostId) // añadir ct una vez se añada la funcion de Token al proyecto (cmd.ParentPostId, ct)
                ?? throw new InvalidOperationException("Post  no encontrado.");

            // No notificar si alguien se hace repost a sí mismo
            if (repostPost.Author.UserId == cmd.ReposterUserId) return;

            await _bus.Publish(new PostRepostedEvent(
                PostId: cmd.RePostId,        // el post que recibió un repost
                AuthorId: repostPost.Author.UserId,     // quien recibe la notificación
                ReposterUserId: cmd.ReposterUserId,  // quien hizo el repost
                ReposterUsername: cmd.ReposterUsername,
                ReposterDisplayName: cmd.ReposterDisplayName,
                ReposterAvatarUrl: cmd.ReposterAvatarUrl,
                ContentPreview: repostPost.Content[..Math.Min(80, repostPost.Content.Length)],
                OccurredAt: DateTimeOffset.UtcNow
            ), ct);
        }
    }
}
