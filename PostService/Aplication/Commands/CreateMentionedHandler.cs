using MassTransit;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace PostService.Aplication.Commands
{
    public class CreateMentionedHandler
    {
        private readonly IPublishEndpoint _bus;
        private readonly IPostRepository _post;

        public CreateMentionedHandler(IPublishEndpoint bus, IPostRepository post)
        {
            _bus = bus;
            _post = post;
        }

        public async Task HandleAsync(CreateMentionedCommand cmd, CancellationToken ct)
        {
            var mentionPost = await _post.GetPostDetailAsync(cmd.MentionPostId) // añadir ct una vez se añada la funcion de Token al proyecto (cmd.ParentPostId, ct)
                ?? throw new InvalidOperationException("Post  no encontrado.");

            // No notificar si alguien se hace repost a sí mismo
            if (mentionPost.Author.UserId == cmd.MentionerUserId) return;

            await _bus.Publish(new PostMentionedEvent(
                PostId: cmd.MentionPostId,        // el post que recibió un repost
                MentionedUserId: mentionPost.Author.UserId,     // quien recibe la notificación
                MentionerUserId: cmd.MentionerUserId,  // quien hizo el repost
                MentionerUsername: cmd.MentionerUsername,
                MentionerDisplayName: cmd.MentionerDisplayName,
                MentionerAvatarUrl: cmd.MentionerAvatarUrl,
                ContentPreview: mentionPost.Content[..Math.Min(80, mentionPost.Content.Length)],
                OccurredAt: DateTimeOffset.UtcNow
            ), ct);
        }
    }
}
