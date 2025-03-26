using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

[Route("api/combined")]
[ApiController]
public class CombinedController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string CohereApiUrl = "https://api.cohere.ai/generate";
    private const string CohereApiKey = "YHe4sRPG52H9jjLK8PGWOqk9YJqECIrdFzhtkWPO";
    private const string StabilityApiUrl = "https://api.stability.ai/v2beta/stable-image/edit/inpaint";
    private const string StabilityApiKey = "sk-Ct1hCixLWTKP7EFviXQMbKVgFgOxKBhV3Q49XuK1t7Tp6lzh";

    public CombinedController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("generate-and-inpaint")]
    public async Task<IActionResult> GenerateAndInpaint([FromForm] CombinedRequestDTO request)
    {
        var cohereClient = _httpClientFactory.CreateClient();
        cohereClient.Timeout = TimeSpan.FromMinutes(5);
        cohereClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CohereApiKey);

        var cohereRequestData = new
        {
            model = "command-xlarge",
            prompt = request.TextPrompt,
            max_tokens = 500,
            temperature = 0.8,
            k = 0,
            p = 0.75,
            stop_sequences = new[] { "END" }
        };

        var cohereContent = new StringContent(JsonSerializer.Serialize(cohereRequestData), Encoding.UTF8, "application/json");
        var cohereResponse = await cohereClient.PostAsync(CohereApiUrl, cohereContent);


        if (!cohereResponse.IsSuccessStatusCode)
        {
            var errorContent = await cohereResponse.Content.ReadAsStringAsync();
            return BadRequest(new { message = "Cohere API Error", details = errorContent });
        }

        var cohereResponseJson = await cohereResponse.Content.ReadAsStringAsync();
        string text = "";
        using (JsonDocument doc = JsonDocument.Parse(cohereResponseJson))
        {
             text = doc.RootElement.GetProperty("text").GetString();
        }

        var stabilityClient = _httpClientFactory.CreateClient();
        stabilityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", StabilityApiKey);
        stabilityClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));

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

            var promptContent = new StringContent(text);
            
            promptContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"prompt\""
            };
            form.Add(promptContent);
            
            var negativePromptContent = new StringContent("European, blonde, blue eyes, western features, pale skin, high nose bridge, oval face, sharp jawline, western makeup style", Encoding.UTF8);
            negativePromptContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"negative_prompt\""
            };
            form.Add(negativePromptContent);

            var outputFormatContent = new StringContent(request.OutputFormat ?? "webp");
            outputFormatContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"output_format\""
            };
            form.Add(outputFormatContent);

            var stabilityResponse = await stabilityClient.PostAsync(StabilityApiUrl, form);

            if (!stabilityResponse.IsSuccessStatusCode)
            {
                var errorContent = await stabilityResponse.Content.ReadAsStringAsync();
                return BadRequest(new { message = "Stability API Error", details = errorContent });
            }

            var contentType = stabilityResponse.Content.Headers.ContentType?.MediaType;

            if (contentType.StartsWith("image/"))
            {
                var resultData = await stabilityResponse.Content.ReadAsByteArrayAsync();
                var base64Image = Convert.ToBase64String(resultData);

                return Ok(new
                {
                    generatedPrompt = text,
                    imageData = $"data:{contentType};base64,{base64Image}"
                });
            }
            else
            {
                var resultString = await stabilityResponse.Content.ReadAsStringAsync();
                return Ok(new
                {
                    generatedPrompt = promptContent,
                    message = "Response is not an image",
                    contentType,
                    resultString
                });
            }
        }
    }
}


