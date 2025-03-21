using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _context.NotificationHistories
            .Where(nh => nh.UserId == userId && nh.IsBlocked == false) 
            .Select(nh => new 
            {
                Id = nh.Id,
                NotificationId = nh.NotificationId,
                UserId = nh.UserId,
                Description = nh.Notification.Description,
                Date = nh.Date,
                Title = nh.Title
            })
            .ToListAsync();

        return notifications.Cast<object>().ToList() ?? new List<object>();
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        var notification = await _context.NotificationHistories
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null) return false;
        notification.IsBlocked = true;
        await _context.SaveChangesAsync();
    
        return true;
    }
}