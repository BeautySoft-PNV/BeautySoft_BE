namespace BeautySoftBE.Models
{
    public class MakeupItemModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Guidance { get; set; }
        public DateTime DateOfManufacture { get; set; }
        public DateTime ExpirationDate { get; set; }
        public UserModel User { get; set; }
        public ICollection<MakeupItemStyleModel> MakeupItemStyles { get; set; }
    }
}