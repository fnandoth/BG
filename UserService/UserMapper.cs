using UserService.Domain.Entities;
using UserService.Aplication.DTOs;

namespace UserService
{
    public static class UserMapper
    {
        // User -> UserResponseDTO
        public static UserResponseDTO ToResponseDTO(this User user) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            IsPrivate = user.IsPrivate,
            CreatedAt = user.CreatedAt
        };

        // UserDTO -> User
        public static User ToEntity(this UserDTO dto) => new()
        {
            UserName = dto.UserName,
            Password = dto.Password,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            AvatarUrl = dto.AvatarUrl,
            Bio = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // UserUpdateDTO -> User (aplica sobre entidad existente)
        public static void ApplyUpdate(this UserUpdateDTO dto, User user)
        {
            user.UserName = dto.UserName;
            user.Password = dto.Password;
            user.Email = dto.Email;
            user.DisplayName = dto.DisplayName;
            user.Bio = dto.Bio;
            user.AvatarUrl = dto.AvatarUrl;
            user.IsPrivate = dto.IsPrivate;
            user.UpdatedAt = dto.UpdatedAt;
        }

        // User -> UserPlainDTO
        public static UserPlainDTO ToPlainDTO(this User user) => new()
        {
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt
        };

        // 
    }
}