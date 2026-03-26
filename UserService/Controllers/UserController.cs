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
            var success = await _userRepository.LoginAsync(user);

            if (!success)
                return Unauthorized(new { message = $"Credenciales incorrectas para el usuario '{user.UserName}'." });

            return Ok(new { message = "Login exitoso." });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO user)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return Ok(updatedUser);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] UserLoginDTO user)
        {
            await _userRepository.DeleteUserAsync(user);
            return NoContent();
        }

        [HttpPost("{followerDisplayName}/follow/{followeeDisplayName}")]
        public async Task<IActionResult> FollowUser(string followerDisplayName, string followeeDisplayName)
        {
            var success = await _userRepository.FollowUserAsync(followerDisplayName, followeeDisplayName);
            if (!success)
                return BadRequest(new { message = $"No se pudo seguir al usuario '{followeeDisplayName}'." });
            return Ok(new { message = $"'{followerDisplayName}' ahora sigue a '{followeeDisplayName}'." });
        }

        [HttpDelete("{followerDisplayName}/unfollow/{followeeDisplayName}")]
        public async Task<IActionResult> UnfollowUser(string followerDisplayName, string followeeDisplayName)
        {
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