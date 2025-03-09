using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManagerStorageController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ManagerStorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-user")]
        public IActionResult CheckUserStorage()
        {
            var userId = GetUserIdFromToken();

            bool exists = _context.ManagerStorages.Any(ms => ms.UserId == userId);

            return Ok(new { status = exists });
        }
    }
}