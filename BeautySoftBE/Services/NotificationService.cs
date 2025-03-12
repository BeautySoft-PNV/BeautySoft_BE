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
            .Where(nh => nh.UserId == userId)
            .Select(nh => new 
            {
                Id = nh.Id,
                NotificationId = nh.NotificationId,
                UserId = nh.UserId,
                Description = nh.Notification.Description,
                Date = nh.Notification.CreateDate,
                Title = nh.Title
            })
            .ToListAsync();


        return notifications.Cast<object>().ToList();
    }
    
    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        var notification = await _context.NotificationHistories
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null) return false;

        _context.NotificationHistories.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

}