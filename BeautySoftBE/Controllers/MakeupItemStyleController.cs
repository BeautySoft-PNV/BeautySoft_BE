using Microsoft.AspNetCore.Mvc;
using BeautySoftBE.Models;
using BeautySoftBE.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupItemStyleController : BaseController
    {
        private readonly IMakeupItemStyleService _makeupItemStyleService;

        public MakeupItemStyleController(IMakeupItemStyleService makeupItemStyleService)
        {
            _makeupItemStyleService = makeupItemStyleService;
        }
        
        [HttpGet("user/me")]
        public async Task<ActionResult<IEnumerable<MakeupItemStyleModel>>> GetMyMakeupItemStyles()
        {
            var userId = GetUserIdFromToken();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token." });
            }

            var makeupItemStyles = await _makeupItemStyleService.GetByUserIdAsync(userId.Value);

            if (makeupItemStyles == null || !makeupItemStyles.Any())
            {
                return NotFound(new { message = "Không tìm thấy phong cách trang điểm nào." });
            }

            return Ok(makeupItemStyles);
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateMakeupItemStyle([FromBody] MakeupItemStyleModel makeupItemStyle)
        {
            if (makeupItemStyle == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }
            
            if (makeupItemStyle.MakeupItemId == 0 || makeupItemStyle.MakeupStyleId == 0)
            {
                return BadRequest("MakeupItemId và MakeupStyleId không được để trống.");
            }
            
            var isValid = await _makeupItemStyleService.ValidateMakeupItemAndStyleAsync(makeupItemStyle.MakeupItemId, makeupItemStyle.MakeupStyleId);
            if (!isValid)
            {
                return NotFound("MakeupItemId hoặc MakeupStyleId không tồn tại.");
            }
            
            var userExists = await _makeupItemStyleService.ValidateUserAsync(makeupItemStyle.UserId);
            if (!userExists)
            {
                return NotFound("UserId không tồn tại trong hệ thống.");
            }
            
            await _makeupItemStyleService.CreateAsync(makeupItemStyle);
            return Ok("MakeupItemStyle được tạo thành công!");
        }
    }
}