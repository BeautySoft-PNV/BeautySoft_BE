using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ImageEditController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ImageEditController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("inpaint")]
    public async Task<IActionResult> InpaintImage([FromForm] ImageEditDTO request)
    {
        if (request.Image == null || request.Image.Length == 0)
        {
            return BadRequest(new { message = "Image file is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new { message = "Prompt is required." });
        }

        var apiUrl = "https://api.stability.ai/v2beta/stable-image/edit/inpaint";
        var apiKey = "sk-x4mSrQK87o5QA16k09q7jPzmAYBFPmwE0oupHBZhpB8MWBaZ";

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));

        using (var form = new MultipartFormDataContent())
        {
            var imageContent = new StreamContent(request.Image.OpenReadStream());
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"image\"",
                FileName = $"\"{request.Image.FileName}\""
            };
            form.Add(imageContent);

            if (request.Mask != null && request.Mask.Length > 0)
            {
                var maskContent = new StreamContent(request.Mask.OpenReadStream());
                maskContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                maskContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"mask\"",
                    FileName = $"\"{request.Mask.FileName}\""
                };
                form.Add(maskContent);
            }

            var promptContent = new StringContent(request.Prompt);
            promptContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"prompt\""
            };
            form.Add(promptContent);

            var outputFormatContent = new StringContent(request.OutputFormat ?? "webp");
            outputFormatContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"output_format\""
            };
            form.Add(outputFormatContent);

            try
            {
                var response = await client.PostAsync(apiUrl, form);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"API Error: {response.StatusCode}", details = errorContent });
                }

                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (contentType.StartsWith("image/"))
                {
                    var resultData = await response.Content.ReadAsByteArrayAsync();
                    return File(resultData, contentType);
                }
                else
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    return Ok(new { message = "Response is not an image", contentType, resultString });
                }

            }
            catch (HttpRequestException e)
            {
                return BadRequest(new { message = $"Request error: {e.Message}" });
            }
        }
    }
}
