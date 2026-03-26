namespace UserService.Aplication.DTOs
{
    public class UserDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public required string AvatarUrl { get; set; }

    }

    public class UserResponseDTO
    {
        public required Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public required string Bio { get; set; }
        public required string AvatarUrl { get; set; }
        public required bool IsPrivate { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserLoginDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class UserPlainDTO
    {
        public required string DisplayName { get; set; }
        public required string AvatarUrl { get; set; }
        public required string Bio { get; set; }
        public required DateTime CreatedAt { get; set; }
    }

    public class UserUpdateDTO
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public required string Bio { get; set; }
        public required string AvatarUrl { get; set; }
        public required bool IsPrivate { get; set; }
        public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
