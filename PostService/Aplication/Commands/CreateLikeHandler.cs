using MassTransit;
using PostService.Domain.Interfaces;
using PostService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace PostService.Aplication.Commands
{
    public class CreateLikeHandler
    {
        private readonly IPublishEndpoint _bus;
        private readonly ILikeRepository _like;
        private readonly IPostRepository _post;

        public CreateLikeHandler(IPublishEndpoint bus, ILikeRepository like, IPostRepository post)
        {
            _bus = bus;
            _like = like;
            _post = post;
        }
        // cmd: es la informacion de la solicitud de creación del like, ct: es el token de cancelación para manejar la operación asincrónica
        public async Task HandleAsync(CreateLikeCommand cmd, CancellationToken ct)
        {
            var post = await _post.GetPostDetailAsync(cmd.PostId) // (cmd.PostId, ct) 
                ?? throw new InvalidOperationException($"No se pudo obtener el post");

            // esto se esta realizando en el repositorio. no recuerdo el motivo de hacerlo aqui 
            //await _like.ToggleLikeAsync(cmd.UserId, cmd.PostId); // (cmd.UserId, cmd.PostId, ct)

            // Publica el evento → MassTransit lo enruta a la cola correcta
            await _bus.Publish(new PostLikedEvent(
                PostId: post.Id,
                AuthorId: post.Author.UserId,
                LikerUserId: cmd.UserId,
                LikerUsername: cmd.Username,
                LikerDisplayName: cmd.DisplayName,
                LikerAvatarUrl: cmd.AvatarUrl,
                ContentPreview: post.Content[..Math.Min(80, post.Content.Length)],
                OccurredAt: DateTimeOffset.UtcNow
            ), ct);
        }
    }

}
