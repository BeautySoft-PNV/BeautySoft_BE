using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ImageEditDTO
{
    [Required]
    public IFormFile Image { get; set; }

    [Required]
    public IFormFile Mask { get; set; }

    [Required]
    public string Prompt { get; set; }

    public string OutputFormat { get; set; } = "webp"; 
}
