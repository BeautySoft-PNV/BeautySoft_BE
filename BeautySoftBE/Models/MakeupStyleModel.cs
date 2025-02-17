namespace BeautySoftBE.Models
{
    public class MakeupStyleModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Guidance { get; set; }
        public UserModel User { get; set; }
        public ICollection<MakeupItemStyleModel> MakeupItemStyles { get; set; }
    }
}