namespace UserService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserName { get; set; }
        public required string Password { get; set; } 
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public required string Bio { get; set; }
        public required string AvatarUrl { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
