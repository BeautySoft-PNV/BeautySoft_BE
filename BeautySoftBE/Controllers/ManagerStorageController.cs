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
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);

            bool exists = _context.Payments.Any(ms => ms.UserId == userId);

            return Ok(new { status = exists });
        }
    }
}