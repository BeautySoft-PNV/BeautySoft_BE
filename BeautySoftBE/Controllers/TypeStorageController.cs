using BeautySoftBE.Data;
using Microsoft.AspNetCore.Mvc;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeStorageController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var typeStorage = await _context.TypeStorages.FindAsync(id);
            if (typeStorage == null)
            {
                return NotFound(new { message = "TypeStorage not found" });
            }
            return Ok(typeStorage);
        }
    }

}