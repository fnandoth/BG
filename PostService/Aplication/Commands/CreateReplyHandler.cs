using MassTransit;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace PostService.Aplication.Commands
{
    public class CreateReplyHandler
    {
        private readonly IPublishEndpoint _bus;
        private readonly IPostRepository _post;

        public CreateReplyHandler(IPublishEndpoint bus, IPostRepository post)
        {
            _bus = bus;
            _post = post;
        }

        public async Task HandleAsync(CreateReplyCommand cmd, CancellationToken ct)
        {
            var parentPost = await _post.GetPostDetailAsync(cmd.ParentPostId) // añadir ct una vez se añada la funcion de Token al proyecto (cmd.ParentPostId, ct)
                ?? throw new InvalidOperationException("Post padre no encontrado.");

            // No notificar si alguien se responde a sí mismo
            if (parentPost.Author.UserId == cmd.ReplierUserId) return;

            // El ContentPreview es del reply nuevo, no del post padre
            var replyPost = await _post.GetPostDetailAsync(cmd.ReplyPostId) // añadir ct una vez se añada la funcion de Token al proyecto (cmd.ReplyPostId, ct)
                ?? throw new InvalidOperationException("Reply no encontrado.");

            await _bus.Publish(new PostRepliedEvent(
                PostId: cmd.ParentPostId,        // el post que recibió la respuesta
                AuthorId: parentPost.Author.UserId,     // quien recibe la notificación
                ReplierUserId: cmd.ReplierUserId,
                ReplierUsername: cmd.ReplierUsername,
                ReplierDisplayName: cmd.ReplierDisplayName,
                ReplierAvatarUrl: cmd.ReplierAvatarUrl,
                ContentPreview: replyPost.Content[..Math.Min(80, replyPost.Content.Length)],
                OccurredAt: DateTimeOffset.UtcNow
            ), ct);
        }
    }
}
