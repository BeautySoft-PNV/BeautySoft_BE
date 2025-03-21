using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Models
{
    public class NotificationHistoryModel
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationId { get; set; }
        public string Title { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public UserModel User { get; set; }
        public NotificationModel Notification { get; set; }
        
        public bool IsBlocked { get; set; } = false;
    }
}