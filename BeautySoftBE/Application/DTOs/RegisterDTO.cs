using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BeautySoftBE.Application.DTOs;

public class RegisterDTO
{
    [CustomUsernameValidation]
    public string Username { get; set; }
    
    [CustomEmailRValidation]
    public string Email { get; set; }
    
    [CustomPasswordRValidation]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "RoleId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0.")]
    public int RoleId { get; set; }
}

public class CustomUsernameValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var username = value as string;
        if (string.IsNullOrEmpty(username))
        {
            return new ValidationResult("Username is required.");
        }
        if (username.Length < 3)
        {
            return new ValidationResult("Username must be at least 3 characters.");
        }
        return ValidationResult.Success;
    }
}

public class CustomEmailRValidation : ValidationAttribute
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

public class CustomPasswordRValidation : ValidationAttribute
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
