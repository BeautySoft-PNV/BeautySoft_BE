namespace BeautifulShop_BE.Models
{
    public class NotificationHistoryModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationId { get; set; }
        public UserModel User { get; set; }
        public NotificationModel Notification { get; set; }
    }
}