namespace BeautySoftBE.Services;

public interface INotificationService
{
    Task<List<object>> GetNotificationsByUserIdAsync(int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
}