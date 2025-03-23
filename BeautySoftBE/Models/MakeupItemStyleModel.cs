using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BeautySoftBE.Models
{
    public class MakeupItemStyleModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "MakeupItemId cannot be empty.")]
        public int MakeupItemId { get; set; }

        [Required(ErrorMessage = "MakeupStyleId cannot be empty.")]
        public int MakeupStyleId { get; set; }

        [Required(ErrorMessage = "UserId cannot be blank.")]
        public int UserId { get; set; }
        
        [ForeignKey(nameof(MakeupItemId))]
        public MakeupItemModel? MakeupItem { get; set; }
        
        [ForeignKey(nameof(MakeupStyleId))]
        public MakeupStyleModel? MakeupStyle { get; set; }
    }
}
