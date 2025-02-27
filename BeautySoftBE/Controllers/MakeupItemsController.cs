using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BeautySoftBE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupItemsController : BaseController
    {
        private readonly IMakeupItemService _makeupItemService;
        private readonly ApplicationDbContext _context;

        public MakeupItemsController(IMakeupItemService makeupItemService, ApplicationDbContext context)
        {
            _makeupItemService = makeupItemService;
            _context = context;
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MakeupItemModel>>> GetMakeupItems()
        {
            return Ok(await _makeupItemService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MakeupItemModel>> GetMakeupItem(int id)
        {
            var makeupItem = await _makeupItemService.GetByIdAsync(id);
            if (makeupItem == null)
            {
                return NotFound(new { message = "Không tìm thấy Makeup Item." });
            }

            return Ok(makeupItem);
        }
        
        [HttpGet("user/me")]
        public async Task<ActionResult<IEnumerable<MakeupItemModel>>> GetMyMakeupItems()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token" });
            }

            var makeupItems = await _makeupItemService.GetByUserIdAsync(userId.Value);
            if (makeupItems == null || !makeupItems.Any())
            {
                return NotFound();
            }

            return Ok(makeupItems);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(MakeupItemModel makeupItem, IFormFile imageFile)
        {
            try
            {
                var createdItem = await _makeupItemService.CreateAsync(makeupItem, imageFile);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> PutMakeupItem(MakeupItemModel makeupItemDto, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token" });
            }

            if (userId.Value != makeupItemDto.UserId)
            {
                return Forbid("Bạn chỉ có thể cập nhật sản phẩm của chính mình.");
            }

            var result = await _makeupItemService.UpdateAsync(makeupItemDto, imageFile);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakeupItem(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định UserId từ token" });
            }

            var makeupItem = await _makeupItemService.GetByIdAsync(id);
            if (makeupItem == null || makeupItem.UserId != userId)
            {
                return NotFound("Không tìm thấy sản phẩm hoặc sản phẩm này không thuộc về bạn.");
            }
            
            var result = await _makeupItemService.DeleteAsync(id);
            if (!result)
            {
                return BadRequest("Không thể xóa sản phẩm.");
            }

            return NoContent();
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MakeupItemModel>>> SearchMakeupItems([FromQuery] string? name)
        {
            var items = await _makeupItemService.SearchByNameAsync(name);
            if (items == null || !items.Any())
            {
                return NotFound("Không tìm thấy sản phẩm phù hợp.");
            }

            return Ok(items);
        }
    }
}
