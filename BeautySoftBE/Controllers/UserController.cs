using System.IdentityModel.Tokens.Jwt;
using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BeautySoftBE.Application.DTOs;
using BeautySoftBE.Data;
using BeautySoftBE.Middleware;
using BeautySoftBE.Repositories;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public UsersController( IUserService userService, IAuthService authService, IUserRepository userRepository)
        {
            _userService = userService;
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserModel>> GetUserFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (userId == null) return Unauthorized("Invalid token.");

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null) return NotFound("User information not found.");

            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUser([FromForm] UserRequestDTO user,[FromForm] String? newPassword,[FromForm] IFormFile? imageFile)
        {
            Console.WriteLine("Image file uploaded: " + imageFile);

            if (user == null)
            {
                return BadRequest("User data is missing.");
            }
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (userId == null) return Unauthorized("Invalid token.");
            var userModel = await _userRepository.GetEmailByUsernameAsync(user.Email);
            if (user.Password != null)
            {
                if (newPassword != null)
                {
                    if (!VerifyPassword(user.Password, userModel.Password))
                    {
                        return NotFound("old password does not match.");
                    }
                }
                else
                {
                    return NotFound("NewPassword null.");
                }
            }

            user.Id = userId.Value;
            var result = await _userService.UpdateAsync(user, newPassword, imageFile);
            if (!result) return NotFound("No user found to update.");

            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (userId == null) return Unauthorized("Invalid Token.");

            var result = await _userService.DeleteAsync(userId.Value);
            if (!result) return NotFound("No user found to delete.");

            return NoContent();
        }
        
        private bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        
        [AdminAuthorize]
        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UserRequestDTO user, [FromForm] string? newPassword, [FromForm] IFormFile? imageFile)
        {
            if (user == null)
            {
                return BadRequest("User data is missing.");
            }
        
            var userModel = await _userRepository.GetEmailByUsernameAsync(user.Email);
            if (user.Password != null)
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    return BadRequest("New password is required.");
                }
                
                if (newPassword.Length < 6)
                {
                    return BadRequest("New password must be at least 6 characters long.");
                }

                if (!VerifyPassword(user.Password, userModel.Password))
                {
                    return BadRequest("Old password does not match.");
                }
            }

            
            user.Id = id;
        
            var result = await _userService.UpdateAsync(user, newPassword, imageFile);
            if (!result) return NotFound("No user found to update.");
        
            return NoContent();
        }
        
        [AdminAuthorize]
        [HttpGet("all")]
        public async Task<ActionResult<List<UserModel>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }
        
        [AdminAuthorize]
        [HttpPut("block/{userId}")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            int userIdInt = int.Parse(userId);
            var result = await _userService.BlockUserAsync(userIdInt);
            if (!result) return NotFound("User not found.");
            return Ok("User has been blocked.");
        }
        
        [AdminAuthorize]
        [HttpPut("unblock/{userId}")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            int userIdInt = int.Parse(userId);
            var result = await _userService.UnblockUserAsync(userIdInt);
            if (!result) return NotFound("User not found.");

            return Ok("User has been unblocked.");
        }

    }
}
