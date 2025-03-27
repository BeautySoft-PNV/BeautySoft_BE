using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
[Route("api/combined")]
[ApiController]
public class CombinedController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string CohereApiUrl = "https://api.cohere.ai/generate";
    private const string CohereApiKey = "n00tI0jSlF4GXqNL9ynk40YAAP9GEd9UrKJYfiuk";
    private const string StabilityApiUrl = "https://api.stability.ai/v2beta/stable-image/edit/inpaint";
    private const string StabilityApiKey = "sk-53ZJHkkEoOkioEYGkWLImplw9MTM8lig0kzZhmaR0oT3wtuf";
   
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
            var negativePromptContent = new StringContent(
                "no altered ethnicity, no skin tone lightening, no European facial features, no Westernization, no Caucasian features, no unrealistic modifications, no altered face, no identity change, no transformed features, no unrealistic face, no Western facial proportions, no light-colored eyes, no blond or light brown hair, no thin nose bridge, no deep-set eyes, no high nose bridge, no narrow lips, no small thin lips, no Western-style eyebrows, only makeup applied, keep original Asian facial structure, maintain Vietnamese appearance, retain Southeast Asian eye shape, retain natural full lips, keep monolid or epicanthic fold eyes, maintain Vietnamese facial balance", 
                Encoding.UTF8
            );
            negativePromptContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"negative_prompt\""
            };

            form.Add(negativePromptContent);
            
            var modelContent = new StringContent("stable-diffusion-xl");
            modelContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"model\""
            };
            form.Add(modelContent);
            
            var guidanceScaleContent = new StringContent("5.0");
            guidanceScaleContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"guidance_scale\""
            };
            form.Add(guidanceScaleContent);


            var outputFormatContent = new StringContent(request.OutputFormat ?? "webp");
            outputFormatContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"output_format\""
            };
            var imageStrengthContent = new StringContent("0.2"); 
            imageStrengthContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"image_strength\""
            };
            form.Add(imageStrengthContent);
                 var denoisingStrengthContent = new StringContent("0.3");
        denoisingStrengthContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"denoising_strength\""
        };
        form.Add(denoisingStrengthContent);
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


