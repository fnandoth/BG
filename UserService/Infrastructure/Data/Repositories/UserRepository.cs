using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UserService.Aplication.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        // ─── Privado: obtener usuario por nombre ──────────────────────────────────

        private async Task<Domain.Entities.User> GetUserEntityByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName)
                ?? throw new InvalidOperationException($"No se pudo encontrar el usuario '{userName}'");
        }

        // ─── Login ────────────────────────────────────────────────────────────────

        public async Task<bool> LoginAsync(UserLoginDTO user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (existingUser == null)
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);
            }
            catch (SaltParseException)
            {
                throw new InvalidOperationException(
                    $"El hash de la contraseña del usuario '{user.UserName}' está corrupto.");
            }
        }

        // ─── Register ─────────────────────────────────────────────────────────────

        public async Task<UserResponseDTO> RegisterAsync(UserDTO user)
        {
            var exists = await _context.Users.AnyAsync(u => u.UserName == user.UserName);
            if (exists)
                throw new InvalidOperationException($"El usuario '{user.UserName}' ya existe.");

            var newUser = user.ToEntity();
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(newUser);
            await SaveChangesAsync();

            return newUser.ToResponseDTO();
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        public async Task<bool> DeleteUserAsync(UserLoginDTO user)
        {
            var trust = await LoginAsync(user);

            if (!trust)
                throw new UnauthorizedAccessException(
                    $"Contraseña incorrecta para el usuario '{user.UserName}'.");

            var userToDelete = await GetUserEntityByNameAsync(user.UserName);

            _context.Users.Remove(userToDelete);
            await SaveChangesAsync();

            return true;
        }

        // ─── Update ───────────────────────────────────────────────────────────────

        public async Task<UserResponseDTO> UpdateUserAsync(UserUpdateDTO user)
        {
            var userToUpdate = await GetUserEntityByNameAsync(user.UserName);

            user.ApplyUpdate(userToUpdate);
            _context.Users.Update(userToUpdate);
            await SaveChangesAsync();

            return userToUpdate.ToResponseDTO();
        }

        // ─── Queries ──────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UserPlainDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.ToPlainDTO()).ToList();
        }

        public async Task<UserPlainDTO> GetUserByNameAsync(string displayName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.DisplayName == displayName)
                ?? throw new InvalidOperationException(
                    $"No se pudo encontrar el usuario con DisplayName '{displayName}'.");

            return user.ToPlainDTO();
        }

        
        // ─── Follow ──────────────────────────────────────────────────────────

        public async Task<bool> FollowUserAsync(string followerDisplayName, string followeeDisplayName)
        {
            var Follower = await GetUserEntityByNameAsync(followerDisplayName);
            var Followee = await GetUserEntityByNameAsync(followeeDisplayName);

            var Follow = new Domain.Entities.Follow
            {
                FollowerId = Follower.Id,
                FollowingId = Followee.Id
            };

            _context.Follows.Add(Follow);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowUserAsync(string followerDisplayName, string followeeDisplayName)
        {
            var Follower = await GetUserEntityByNameAsync(followerDisplayName);
            var Followee = await GetUserEntityByNameAsync(followeeDisplayName);

            var Follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == Follower.Id && f.FollowingId == Followee.Id)
                ?? throw new InvalidOperationException(
                    $"No se encontró una relación de seguimiento entre '{followerDisplayName}' y '{followeeDisplayName}'.");

            _context.Follows.Remove(Follow);
            await SaveChangesAsync();

            return true;

        }

        public async Task<IEnumerable<UserPlainDTO>> GetFollowersAsync(string displayName)
        {
            var user = await GetUserEntityByNameAsync(displayName);

            var followers = await _context.Follows
                .Where(f => f.FollowingId == user.Id)
                .Join(
                    _context.Users,
                    follow => follow.FollowerId,
                    user => user.Id,
                    (follow, user) => user
                )
                .ToListAsync();
            //TODO: Agregar paginación y manejo de errores (ej: si el usuario no existe, si no tiene seguidores, etc.)
            return followers.Select(u => u.ToPlainDTO());
        }

        // ─── SaveChanges ──────────────────────────────────────────────────────────

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}