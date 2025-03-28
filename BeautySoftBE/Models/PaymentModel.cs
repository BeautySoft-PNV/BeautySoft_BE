using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Models
{
    public class PaymentModel
    {
        [Key]
        public int Id { get; set; } 
        public int UserId { get; set; }
        public int TypeStorageId { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public UserModel User { get; set; }
        public TypeStorageModel TypeStorage { get; set; }
    }
}