using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Aplication.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{displayName}")]
        public async Task<IActionResult> GetUserByName(string displayName)
        {
            var user = await _userRepository.GetUserByNameAsync(displayName);
            return Ok(user);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            var createdUser = await _userRepository.RegisterAsync(user);
            return CreatedAtAction(nameof(GetUserByName), new { displayName = createdUser.DisplayName }, createdUser);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO user)
        {
            var auth = await _userRepository.LoginAsync(user);

            if (auth is null)
                return Unauthorized(new { message = $"Credenciales incorrectas para el usuario '{user.UserName}'." });

            return Ok(auth);
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var displayName = User.FindFirstValue("display_name");
            var avatarUrl = User.FindFirstValue("avatar_url");

            return Ok(new
            {
                id,
                userName,
                displayName,
                avatarUrl
            });
        }


        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO user)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return Ok(updatedUser);
        }


        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] UserLoginDTO user)
        {
            await _userRepository.DeleteUserAsync(user);
            return NoContent();
        }

        [Authorize]
        [HttpPost("follow/{followeeDisplayName}")]
        public async Task<IActionResult> FollowUser(string followeeDisplayName)
        {
            var followerDisplayName = User.FindFirstValue("display_name")
                ?? throw new UnauthorizedAccessException("El token no contiene el claim 'display_name'.");

            var success = await _userRepository.FollowUserAsync(followerDisplayName, followeeDisplayName);
            if (!success)
                return BadRequest(new { message = $"No se pudo seguir al usuario '{followeeDisplayName}'." });
            return Ok(new { message = $"'{followerDisplayName}' ahora sigue a '{followeeDisplayName}'." });
        }

        [Authorize]
        [HttpDelete("unfollow/{followeeDisplayName}")]
        public async Task<IActionResult> UnfollowUser(string followeeDisplayName)
        {
            var followerDisplayName = User.FindFirstValue("display_name")
                ?? throw new UnauthorizedAccessException("El token no contiene el claim 'display_name'.");

            var success = await _userRepository.UnfollowUserAsync(followerDisplayName, followeeDisplayName);
            if (!success)
                return BadRequest(new { message = $"No se pudo dejar de seguir al usuario '{followeeDisplayName}'." });
            return Ok(new { message = $"'{followerDisplayName}' ha dejado de seguir a '{followeeDisplayName}'." });
        }

        [HttpGet("{displayName}/followers")]
        public async Task<IActionResult> GetFollowers(string displayName)
        {
            var followers = await _userRepository.GetFollowersAsync(displayName);
            return Ok(followers);
        }
    }
}