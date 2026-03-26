namespace UserService.Domain.Entities
{
    // TODO: añadir el uso de tokens una vez se tenga gran parte del proyecto completado
    public class RefreshTokens
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public required string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
