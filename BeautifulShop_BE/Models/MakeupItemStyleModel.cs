namespace BeautifulShop_BE.Models
{
    public class MakeupItemStyleModel
    {
        public int Id { get; set; }
        public int MakeupItemId { get; set; }
        public int MakeupStyleId { get; set; }
        //public MakeupItemModel MakeupItem { get; set; }
        public MakeupStyleModel MakeupStyle { get; set; }
    }
}
