using System.ComponentModel.DataAnnotations;

namespace BeautySoftBE.Application.DTOs;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
}