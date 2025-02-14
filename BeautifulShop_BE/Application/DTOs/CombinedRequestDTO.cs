using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class CombinedRequestDTO
{
    [Required]
    public string TextPrompt { get; set; }

    [Required]
    public IFormFile Image { get; set; }

    [Required]
    public IFormFile Mask { get; set; }

    public string? OutputFormat { get; set; } = "webp";
}