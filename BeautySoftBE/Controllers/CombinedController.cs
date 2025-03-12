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
    private const string CohereApiKey = "k1gbkCVHtgXASIb1Xl0Z9ojo9EArSfW8OrO10YeV";
    private const string StabilityApiUrl = "https://api.stability.ai/v2beta/stable-image/edit/inpaint";
    private const string StabilityApiKey = "sk-MqjP55RO2PtGLdSaBLSb2kCZgkteEiUpJF2SjK8u94iJq5j5";

    public CombinedController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("generate-and-inpaint")]
    public async Task<IActionResult> GenerateAndInpaint([FromForm] CombinedRequestDTO request)
    {
        var cohereClient = _httpClientFactory.CreateClient();
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
        
        string translatedText = await TranslateText(text, "vi", "en");
        var generatedPrompt = translatedText;

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

            var promptContent = new StringContent(generatedPrompt);
            
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
                string finalTranslatedText = await TranslateText(generatedPrompt, "en", "vi");

                return Ok(new
                {
                    generatedPrompt = finalTranslatedText,
                    imageData = $"data:{contentType};base64,{base64Image}"
                });
            }
            else
            {
                var resultString = await stabilityResponse.Content.ReadAsStringAsync();
                return Ok(new
                {
                    generatedPrompt = generatedPrompt,
                    message = "Response is not an image",
                    contentType,
                    resultString
                });
            }
        }
    }
    private async Task<string> TranslateText(string text, string sourceLang, string targetLang)
    {
        var client = new RestClient("https://api.mymemory.translated.net/get");
        int maxChunkSize = 500;
        List<string> translatedParts = new List<string>();

        for (int i = 0; i < text.Length; i += maxChunkSize)
        {
            string chunk = text.Substring(i, Math.Min(maxChunkSize, text.Length - i));
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddParameter("q", chunk);
            request.AddParameter("langpair", $"{sourceLang}|{targetLang}");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var json = JsonDocument.Parse(response.Content);
                string translatedChunk = json.RootElement.GetProperty("responseData").GetProperty("translatedText").GetString();
                translatedParts.Add(translatedChunk);
            }
        }

        return string.Join(" ", translatedParts);
    }

}


