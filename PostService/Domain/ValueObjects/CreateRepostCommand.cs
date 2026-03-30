namespace PostService.Domain.ValueObjects
{
    public record CreateRepostCommand
    (
        Guid RePostId, // el post que recibió un repost
        Guid AuthorId, // quien recibe la notificación
        Guid ReposterUserId,  // quien hizo el repost
        string ReposterUsername,
        string ReposterDisplayName,
        string ReposterAvatarUrl
    );
}
