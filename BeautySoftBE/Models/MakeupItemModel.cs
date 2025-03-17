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
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid image URL format.")]
        public string? Image { get; set; }

        [StringLength(1000, ErrorMessage = "Instructions must not be longer than 1000 characters.")]
        public string Guidance { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/9999", ErrorMessage = "Invalid production date.")]
        public DateTime DateOfManufacture { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/9999", ErrorMessage = "Invalid expiration date.")]
        public DateTime ExpirationDate { get; set; }
        public UserModel? User { get; set; }
        public ICollection<MakeupItemStyleModel>? MakeupItemStyles { get; set; }
        
        public bool IsValidExpirationDate()
        {
            return ExpirationDate > DateOfManufacture;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExpirationDate <= DateOfManufacture)
            {
                yield return new ValidationResult("Expiration date must be after manufacture date.", new[] { nameof(ExpirationDate) });
            }
        }
    }
    
}