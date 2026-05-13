using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace YnYMono.Services;

public class CloudGeminiProvider : IAiProvider
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public CloudGeminiProvider(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["AiSettings:GeminiApiKey"]!;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent?key={_apiKey}";
        
        var payload = new { content = new { parts = new[] { new { text = text } } } };
        var response = await _http.PostAsJsonAsync(url, payload);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        
        // Extract the float[] from Google's JSON response
        var embeddingArray = json["embedding"]?["values"]?.AsArray();
        return embeddingArray!.Select(x => (float)x!.GetValue<double>()).ToArray();
    }

    public async Task<string> GenerateTextAsync(string systemPrompt, string userPrompt)
    {
        // Using the exact model requested
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemma-4-26b-a4b-it:generateContent?key={_apiKey}";
        
        var payload = new { 
            contents = new[] { 
                new { role = "user", parts = new[] { new { text = $"{systemPrompt}\n\n{userPrompt}" } } } 
            } 
        };
        
        var response = await _http.PostAsJsonAsync(url, payload);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        
        return json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "AI Error";
    }
}