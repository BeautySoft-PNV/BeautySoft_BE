using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Url(ErrorMessage = "Định dạng URL Avatar không hợp lệ.")]
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "RoleId không được bỏ trống.")]
        public int RoleId { get; set; }
        public RoleModel? Role { get; set; }
    }
}