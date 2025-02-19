using Microsoft.AspNetCore.Mvc;
using BeautySoftBE.Models;
using BeautySoftBE.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupStylesController : ControllerBase
    {
        private readonly IMakeupStyleService _makeupStyleService;

        public MakeupStylesController(IMakeupStyleService makeupStyleService)
        {
            _makeupStyleService = makeupStyleService;
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
        public async Task<ActionResult<MakeupStyleModel>> PostMakeupStyle(MakeupStyleModel makeupStyle)
        {
            await _makeupStyleService.CreateAsync(makeupStyle);
            return CreatedAtAction(nameof(GetMakeupStyle), new { id = makeupStyle.Id }, makeupStyle);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMakeupStyle(int id, MakeupStyleModel makeupStyle)
        {
            if (id != makeupStyle.Id)
            {
                return BadRequest();
            }

            await _makeupStyleService.UpdateAsync(makeupStyle);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakeupStyle(int id)
        {
            await _makeupStyleService.DeleteAsync(id);
            return NoContent();
        }
        
        [HttpGet("user/me")]
        public async Task<ActionResult<IEnumerable<MakeupStyleModel>>> GetMyMakeupStyles()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            var makeupStyles = await _makeupStyleService.GetByUserIdAsync(userId);

            if (makeupStyles == null || !makeupStyles.Any())
            {
                return NotFound();
            }

            return Ok(makeupStyles);
        }
    }
}
