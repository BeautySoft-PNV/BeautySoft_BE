using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeautySoftBE.Data;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupItemsController : ControllerBase
    {
        private readonly IMakeupItemService _makeupItemService;
        private readonly ApplicationDbContext _context;

        public MakeupItemsController(IMakeupItemService makeupItemService, ApplicationDbContext context)
        {
            _makeupItemService = makeupItemService;
            _context = context;
        }

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
                return NotFound();
            }
            return Ok(makeupItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(MakeupItemModel makeupItem)
        {
            if (makeupItem == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == makeupItem.UserId);
            if (!userExists)
            {
                return BadRequest("UserId không tồn tại trong hệ thống.");
            }

            _context.MakeupItems.Add(makeupItem);
            await _context.SaveChangesAsync();
            return Ok(makeupItem);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMakeupItem(int id, MakeupItemModel makeupItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != makeupItemDto.Id)
            {
                return BadRequest();
            }

            var result = await _makeupItemService.UpdateAsync(makeupItemDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMakeupItem(int id)
        {
            var result = await _makeupItemService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}