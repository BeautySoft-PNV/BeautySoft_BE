using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySoftBE.Models
{
    public class MakeupStyleModel
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public string? Image { get; set; } 

        public string Guidance { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public UserModel? User { get; set; }
        public ICollection<MakeupItemStyleModel>? MakeupItemStyles { get; set; }
    }
}