using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BeautySoftBE.Models
{
    public class MakeupItemStyleModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "MakeupItemId không được để trống.")]
        public int MakeupItemId { get; set; }

        [Required(ErrorMessage = "MakeupStyleId không được để trống.")]
        public int MakeupStyleId { get; set; }

        [Required(ErrorMessage = "UserId không được để trống.")]
        public int UserId { get; set; }
        
        [ForeignKey(nameof(MakeupItemId))]
        public MakeupItemModel? MakeupItem { get; set; }
        
        [ForeignKey(nameof(MakeupStyleId))]
        public MakeupStyleModel? MakeupStyle { get; set; }
    }
}
