using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/cohere")]
[ApiController]
public class CohereController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.cohere.ai/generate";
    private const string ApiKey = "c16bERXascAbVHtGBbsuT0pKgVyA7CVnpst4paXM"; 

    public CohereController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateText([FromBody] CohereDTO request)
    {
        var requestData = new
        {
            model = "command-xlarge-nightly",
            prompt = request.Prompt,
            max_tokens = 3000,
            temperature = 0.8,
            k = 0,
            p = 0.75,
            stop_sequences = new[] { "END" }
        };

        var jsonRequest = JsonSerializer.Serialize(requestData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiKey);

        var response = await _httpClient.PostAsync(ApiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Ok(JsonSerializer.Deserialize<object>(jsonResponse));
        }
        else
        {
            return StatusCode((int)response.StatusCode, jsonResponse);
        }
    }
}