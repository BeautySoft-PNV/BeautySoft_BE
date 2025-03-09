using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySoftBE.Models
{
    public class MakeupStyleModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId không được để trống.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống.")]
        [StringLength(255, ErrorMessage = "URL ảnh không được vượt quá 255 ký tự.")]
        public string? Image { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hướng dẫn không được để trống.")]
        [StringLength(500, ErrorMessage = "Hướng dẫn không được vượt quá 500 ký tự.")]
        public string Guidance { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public UserModel? User { get; set; }
        public ICollection<MakeupItemStyleModel>? MakeupItemStyles { get; set; }
    }
}