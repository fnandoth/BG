using UserService.Aplication.DTOs;
using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces
{
    public interface IUserRepository
    { 
        Task<User> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserPlainDTO>> GetAllUsersAsync();
        Task<UserPlainDTO> GetUserByNameAsync(string displayName);
        Task<UserResponseDTO> RegisterAsync(UserDTO user);
        Task<bool> LoginAsync(UserLoginDTO user);
        Task<bool> FollowUserAsync(string followerDisplayName, string followeeDisplayName);
        Task<bool> UnfollowUserAsync(string followerDisplayName, string followeeDisplayName);
        Task<IEnumerable<UserPlainDTO>> GetFollowersAsync(string displayName);
        Task<UserResponseDTO> UpdateUserAsync(UserUpdateDTO user);
        Task<bool> DeleteUserAsync(UserLoginDTO user);
        Task<int> SaveChangesAsync(); // test (xd)
    }
}
