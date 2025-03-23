using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BeautySoftBE.Application.DTOs;

public class UserRequestDTO
{
    public int Id { get; set; }
        
    [CustomNameValidation]
    public string Name { get; set; }
        
    [CustomEmailValidation]
    public string Email { get; set; }
    
    public string? Password { get; set; }

    [Url(ErrorMessage = "Định dạng URL Avatar không hợp lệ.")]
    public string? Avatar { get; set; }

    [Required(ErrorMessage = "RoleId không được bỏ trống.")]
    public int RoleId { get; set; }
     public class CustomNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
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