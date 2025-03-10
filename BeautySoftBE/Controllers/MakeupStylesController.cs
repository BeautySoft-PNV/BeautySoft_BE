using Microsoft.AspNetCore.Mvc;
using BeautySoftBE.Models;
using BeautySoftBE.Services;
using System.IdentityModel.Tokens.Jwt;
using BeautySoftBE.Data;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupStylesController : BaseController 
    {
        private readonly IMakeupStyleService _makeupStyleService;
        private readonly ApplicationDbContext _context;

        public MakeupStylesController(IMakeupStyleService makeupStyleService, ApplicationDbContext context)
        {
            _makeupStyleService = makeupStyleService;
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MakeupStyleModel>>> GetMakeupStyles()
        {
            var makeupStyles = await _makeupStyleService.GetAllAsync();
            return Ok(makeupStyles);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<MakeupStyleModel>> GetMakeupStyle(int id)
        {
            var makeupStyle = await _makeupStyleService.GetByIdAsync(id);

            if (makeupStyle == null)
            {
                return NotFound();
            }

            return Ok(makeupStyle);
        }
        
        [HttpPost]
        public async Task<ActionResult<MakeupStyleModel>> PostMakeupStyle([FromForm] MakeupStyleModel makeup,[FromForm] IFormFile imageFile)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
             if(userId != null){ makeup.UserId = userId.Value;}
            
            bool userHasStorage = await _context.ManagerStorages.AnyAsync(ms => ms.UserId == userId);

            if (!userHasStorage)
            {
                int itemCount = await _context.MakeupStyles.CountAsync(m => m.UserId == userId);
        
                if (itemCount >= 20)
                {
                    return BadRequest("Storage limit reached, maximum 20 products can be stored.");
                }
            }

            await _makeupStyleService.CreateAsync(makeup, imageFile);
            return CreatedAtAction(nameof(GetMakeupStyle), new { id = makeup.Id }, makeup);
        }

        /*
        [HttpPut("update")]
        public async Task<IActionResult> PutMakeupStyle([FromBody] MakeupStyleModel makeupStyle, IFormFile imageFile)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token" });
            }
            
            var existingStyle = await _makeupStyleService.GetByIdAsync(makeupStyle.Id);
            if (existingStyle == null || existingStyle.UserId != userId)
            {
                return NotFound(new { message = "MakeupStyle không tồn tại hoặc không thuộc về người dùng này" });
            }
            
            await _makeupStyleService.UpdateAsync(makeupStyle, imageFile);
            return NoContent();
        }
        */
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakeupStyle(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token." });
            }
            
            var makeupStyle = await _makeupStyleService.GetByIdAsync(id);
            if (makeupStyle == null)
            {
                return NotFound("Không tìm thấy phong cách trang điểm.");
            }
            
            if (makeupStyle.UserId != userId)
            {
                return Forbid("Phong cách này không thuộc về bạn.");
            }
            
            await _makeupStyleService.DeleteAsync(id);

            return NoContent();
        }
        
        [HttpGet("user/me")]
        public async Task<ActionResult<IEnumerable<MakeupStyleModel>>> GetMyMakeupStyles()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token" });
            }

            var makeupStyles = await _makeupStyleService.GetByUserIdAsync(userId.Value);
            if (makeupStyles == null || !makeupStyles.Any())
            {
                return NotFound(new { message = "Không tìm thấy phong cách trang điểm nào" });
            }
            return Ok(makeupStyles);
        }
        
        public int? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            
            if (!handler.CanReadToken(token))
            {
                Console.WriteLine("Token không hợp lệ hoặc bị hỏng.");
                return null;
            }

            var jsonToken = handler.ReadJwtToken(token);

            var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
    
            return userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;
        }

    }
}
