using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Models
{
    public class TypeStorageModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
    }
}