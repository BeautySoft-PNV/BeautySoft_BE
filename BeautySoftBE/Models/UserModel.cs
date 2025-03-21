using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace BeautySoftBE.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        
        [CustomNameValidation]
        public string Name { get; set; }
        
        [CustomEmailValidation]
        public string Email { get; set; }
        
        [CustomPasswordValidation]
        public string Password { get; set; }

        [Url(ErrorMessage = "Invalid Avatar URL format.")]
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "RoleId cannot be empty.")]
        public int RoleId { get; set; }
        public RoleModel? Role { get; set; }
        public bool IsBlocked { get; set; } = false;
    }
    public class CustomNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;
            if (string.IsNullOrEmpty(name))
            {
                return new ValidationResult("Name cannot be left blank.");
            }
            if (name.Length > 100)
            {
                return new ValidationResult("Name cannot exceed 100 characters.");
            }
            return ValidationResult.Success;
        }
    }
    public class CustomEmailValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            if (string.IsNullOrEmpty(email))
            {
                return new ValidationResult("Email cannot be left blank.");
            }
            if (email.Length > 150)
            {
                return new ValidationResult("Email must not exceed 150 characters.");
            }
            
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return new ValidationResult("Invalid Email Format.");
            }
            return ValidationResult.Success;
        }
    }
    public class CustomPasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password cannot be left blank.");
            }
            if (password.Length < 6)
            {
                return new ValidationResult("Password must be at least 6 characters long.");
            }
            return ValidationResult.Success;
        }
    }
}
