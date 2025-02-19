using System.ComponentModel.DataAnnotations;
using BeautySoftBE.Models;

namespace BeautySoftBE.Application.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "RoleId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0.")]
    public int RoleId { get; set; }
}