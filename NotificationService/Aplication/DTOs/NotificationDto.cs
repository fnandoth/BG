namespace NotificationService.Aplication.DTOs
{
    public record NotificationDto(
        Guid Id,
        string Type,           // -> "like", "follow", "reply", etc.
        bool IsRead,
        DateTimeOffset CreatedAt,
        ActorDto Actor,          // quién hizo la acción
        PostPreviewDto? Post     // el post involucrado (null si es un follow/mensaje)
    );

    public record ActorDto(
        Guid UserId,
        string Username,
        string DisplayName,
        string? AvatarUrl
    );

    public record PostPreviewDto(
        Guid PostId,
        string ContentPreview    // primeros ~80 chars del post para tener una menor carga de datos en la notificación (si aplica)
    );

    // paginacion de resultados. podria cargarlo en otra parte para que sea mas reutilizable, pero por ahora lo dejo aquí
    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
    }
}
