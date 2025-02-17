using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Models
{
    public class MakeupItemModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Tên không được dài hơn 100 ký tự.")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Mô tả không được dài hơn 500 ký tự.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Định dạng URL hình ảnh không hợp lệ.")]
        public string Image { get; set; }

        [StringLength(1000, ErrorMessage = "Hướng dẫn không được dài hơn 1000 ký tự.")]
        public string Guidance { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/9999", ErrorMessage = "Ngày sản xuất không hợp lệ.")]
        public DateTime DateOfManufacture { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/9999", ErrorMessage = "Ngày hết hạn không hợp lệ.")]
        public DateTime ExpirationDate { get; set; }
        public UserModel? User { get; set; }
        public ICollection<MakeupItemStyleModel>? MakeupItemStyles { get; set; }
        
        public bool IsValidExpirationDate()
        {
            return ExpirationDate > DateOfManufacture;
        }
    }
}