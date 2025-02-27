namespace BeautySoftBE.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SendDateTime { get; set; }
        public ICollection<NotificationHistoryModel> NotificationHistories { get; set; }
    }
}