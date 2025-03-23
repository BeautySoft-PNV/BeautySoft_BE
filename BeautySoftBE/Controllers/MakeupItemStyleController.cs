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
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Unable to determine UserId from token." });
            }

            var makeupItemStyles = await _makeupItemStyleService.GetByUserIdAsync(userId.Value);

            if (makeupItemStyles == null || !makeupItemStyles.Any())
            {
                return NotFound(new { message = "No makeup styles found." });
            }

            return Ok(makeupItemStyles);
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateMakeupItemStyle([FromBody] MakeupItemStyleModel makeupItemStyle)
        {
            if (makeupItemStyle == null)
            {
                return BadRequest("Invalid data.");
            }
            
            if (makeupItemStyle.MakeupItemId == 0 || makeupItemStyle.MakeupStyleId == 0)
            {
                return BadRequest("MakeupItemId and MakeupStyleId cannot be empty.");
            }
            
            var isValid = await _makeupItemStyleService.ValidateMakeupItemAndStyleAsync(makeupItemStyle.MakeupItemId, makeupItemStyle.MakeupStyleId);
            if (!isValid)
            {
                return NotFound("MakeupItemId or MakeupStyleId does not exist.");
            }
            
            var userExists = await _makeupItemStyleService.ValidateUserAsync(makeupItemStyle.UserId);
            if (!userExists)
            {
                return NotFound("UserId does not exist in the system.");
            }
            
            await _makeupItemStyleService.CreateAsync(makeupItemStyle);
            return Ok("MakeupItemStyle created successfully!");
        }
    }
}