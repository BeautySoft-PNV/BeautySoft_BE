using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserModel>> GetUserFromToken()
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized("Token không hợp lệ.");

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null) return NotFound("Không tìm thấy thông tin người dùng.");

            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel user)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized("Token không hợp lệ.");

            user.Id = userId.Value;
            var result = await _userService.UpdateAsync(user);
            if (!result) return NotFound("Không tìm thấy người dùng để cập nhật.");

            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized("Token không hợp lệ.");

            var result = await _userService.DeleteAsync(userId.Value);
            if (!result) return NotFound("Không tìm thấy người dùng để xóa.");

            return NoContent();
        }
    }
}
