using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[Route("AI")]
public class AIController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;


    public AIController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }
    [HttpGet("Chat")]
    public IActionResult Chat()
    {

        return View(); // will return Views/AI/Chat.cshtml
    }

    [HttpPost("Ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        var apiKey = _config["OpenRouter:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return BadRequest("Missing OpenRouter API key.");

        if (string.IsNullOrWhiteSpace(request?.Question))
            return BadRequest("Question is empty.");

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            model = "openchat/openchat-7b", // or try gpt-3.5, claude, etc.
            messages = new[]
            {
            new { role = "system", content = "You are a helpful AI travel assistant for Cairo tourism." },
            new { role = "user", content = request.Question }
        }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

        var raw = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine("OpenRouter ERROR: " + errorDetails); // 👈 LOG IT
            return StatusCode((int)response.StatusCode, $"OpenRouter error: {errorDetails}");
        }


        var jsonDoc = JsonDocument.Parse(raw);
        var answer = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return Json(new { answer });
    }


}


public class ChatRequest
{
    public string Question { get; set; }
}
