using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace BeautySoftBE.Models
{
    public class UserModel : IdentityUser
    {
        public int Id { get; set; }
        
        [CustomNameValidation]
        public string Name { get; set; }
        
        [CustomEmailValidation]
        public string Email { get; set; }
        
        [CustomPasswordValidation]
        public string Password { get; set; }

        [Url(ErrorMessage = "Định dạng URL Avatar không hợp lệ.")]
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "RoleId không được bỏ trống.")]
        public int RoleId { get; set; }
        public RoleModel? Role { get; set; }
        
        public override string NormalizedUserName { get; set; }
        public override string NormalizedEmail { get; set; }
        public override bool EmailConfirmed { get; set; }
        public override string PasswordHash { get; set; }
        public override string SecurityStamp { get; set; }
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public override string PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }
        public override bool TwoFactorEnabled { get; set; }
        public override int AccessFailedCount { get; set; }
        public override bool LockoutEnabled { get; set; }
        public override DateTimeOffset? LockoutEnd { get; set; }
    }
    public class CustomNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;
            if (string.IsNullOrEmpty(name))
            {
                return new ValidationResult("Tên không được bỏ trống.");
            }
            if (name.Length > 100)
            {
                return new ValidationResult("Tên không được vượt quá 100 ký tự.");
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
                return new ValidationResult("Email không được bỏ trống.");
            }
            if (email.Length > 150)
            {
                return new ValidationResult("Email không được vượt quá 150 ký tự.");
            }
            
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return new ValidationResult("Định dạng Email không hợp lệ.");
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
                return new ValidationResult("Mật khẩu không được bỏ trống.");
            }
            if (password.Length < 6)
            {
                return new ValidationResult("Mật khẩu phải có ít nhất 6 ký tự.");
            }
            return ValidationResult.Success;
        }
    }
}
