using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BeautySoftBE.Application.DTOs;

public class LoginDTO
{
    [CustomEmailValidation]
    public string Email { get; set; }
    
    [CustomPasswordValidation]
    public string Password { get; set; }
}

public class CustomPasswordValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var password = value as string;
        if (string.IsNullOrEmpty(password))
        {
            return new ValidationResult("Password is required.");
        }
        if (password.Length < 6)
        {
            return new ValidationResult("Password must be at least 6 characters.");
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
            return new ValidationResult("Email is required.");
        }
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            return new ValidationResult("Invalid email format.");
        }
        return ValidationResult.Success;
    }
}