using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeautySoftBE.Services;
using BeautySoftBE.Models;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("user/notification")]
        public async Task<IActionResult> GetNotificationsByUserId()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (!userId.HasValue)
            {
                return BadRequest("Không tìm thấy ID người dùng hợp lệ.");
            }

            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId.Value);

            if (notifications == null || notifications.Count == 0)
            {
                return Ok(new List<object>()); 
            }

            return Ok(notifications);
        }
        
        [HttpDelete("user/notification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            Console.WriteLine(notificationId);
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }

            var userId = GetUserIdFromToken(token);
            if (!userId.HasValue)
            {
                return BadRequest("Không tìm thấy ID người dùng hợp lệ.");
            }

            var success = await _notificationService.DeleteNotificationAsync(notificationId, userId.Value);
            if (!success)
            {
                return NotFound("Không tìm thấy thông báo để xóa.");
            }

            return Ok(new { message = "Xóa thông báo thành công." });
        }

    }
}