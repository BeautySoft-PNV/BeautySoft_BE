using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BeautySoftBE.Data;
using BeautySoftBE.Repositories;
using RestSharp.Authenticators;

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
        var userRepository = (IUserRepository)validationContext.GetService(typeof(IUserRepository));
        if (userRepository == null)
        {
            throw new InvalidOperationException("IUserRepository is not available.");
        }

        if (userRepository.UserExistsAsync(email).Result != false)
        {
            
            return new ValidationResult($"Email already exists.");
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
        if (password.Length < 10)
        {
            return new ValidationResult("Password must be at least 10 characters long.");
        }
        if (!Regex.IsMatch(password, @"[A-Z]")) 
        {
            return new ValidationResult("Password must contain at least one uppercase letter.");
        }
        if (!Regex.IsMatch(password, @"[a-z]")) 
        {
            return new ValidationResult("Password must contain at least one lowercase letter.");
        }
        if (!Regex.IsMatch(password, @"\d")) 
        {
            return new ValidationResult("Password must contain at least one digit.");
        }
        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            return new ValidationResult("Password must contain at least one special character.");
        }

        return ValidationResult.Success;
    }
}
