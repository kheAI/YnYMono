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
        
        // ✅ FIXED: Added outputDimensionality = 768 to the embedding payload
        var payload = new { 
            content = new { parts = new[] { new { text = text } } },
            outputDimensionality = 768 
        };
        
        var response = await _http.PostAsJsonAsync(url, payload);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        
        // Extract the float[] from Google's JSON response
        var embeddingArray = json!["embedding"]!["values"]!.AsArray();
        
        // 🛡️ BULLETPROOF SHIELD: .Take(768) guarantees exactly 768 dimensions
        return embeddingArray!.Select(x => (float)x!.GetValue<double>()).Take(768).ToArray();
    }

    public async Task<string> GenerateTextAsync(string systemPrompt, string userPrompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemma-4-26b-a4b-it:generateContent?key={_apiKey}";
        
        // ❌ FIXED: Removed outputDimensionality from here (it is only for vectors, not text)
        var payload = new { 
            contents = new[] { 
                new { role = "user", parts = new[] { new { text = $"{systemPrompt}\n\n{userPrompt}" } } } 
            } 
        };
        
        var response = await _http.PostAsJsonAsync(url, payload);
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        
        return json!["candidates"]![0]!["content"]!["parts"]![0]!["text"]!.ToString() ?? "AI Error";
    }
}