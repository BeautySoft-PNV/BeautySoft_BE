using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupItemsController : ControllerBase
    {
        private readonly IMakeupItemService _makeupItemService;

        public MakeupItemsController(IMakeupItemService makeupItemService)
        {
            _makeupItemService = makeupItemService;
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
        public async Task<ActionResult<MakeupItemModel>> PostMakeupItem(MakeupItemModel makeupItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdItem = await _makeupItemService.CreateAsync(makeupItemDto); // Get the created item from the service
            return CreatedAtAction("GetMakeupItem", new { id = createdItem.Id }, createdItem); // Return the created item
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