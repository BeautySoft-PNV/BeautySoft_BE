using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Application.DTOs;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}