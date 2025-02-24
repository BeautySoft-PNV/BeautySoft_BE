namespace BeautySoftBE.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int TypeStorageId { get; set; }
        public int UserId { get; set; }
        
        public decimal Price { get; set; }
        
        public DateTime PaymentDate { get; set; }
        public TypeStorageModel TypeStorage { get; set; }
        public UserModel User { get; set; }
    }
}