using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace YnYMono.Services;

public class LocalGemmaProvider : IAiProvider
{
    private readonly HttpClient _http;
    
    // The default Ollama port on your Mac/PC
    private readonly string _ollamaUrl = "http://localhost:11434"; 
    private readonly string _textModel = "gemma"; // Update if your local model is named differently
    private readonly string _embedModel = "nomic-embed-text";

    // We inject HttpClient just like we did in CloudGeminiProvider!
    public LocalGemmaProvider(HttpClient http)
    {
        _http = http;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var url = $"{_ollamaUrl}/api/embeddings";
        var payload = new { model = _embedModel, prompt = text };
        
        // Make the HTTP request to local Ollama
        var response = await _http.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();
        
        // Parse the JSON array
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        var embeddingArray = json!["embedding"]!.AsArray();
        
        return embeddingArray.Select(x => (float)x!.GetValue<double>()).ToArray();
    }

    public async Task<string> GenerateTextAsync(string systemPrompt, string userPrompt)
    {
        var url = $"{_ollamaUrl}/api/generate";
        
        // We set stream = false so Ollama gives us the full answer at once
        var payload = new 
        { 
            model = _textModel, 
            prompt = $"{systemPrompt}\n\n{userPrompt}",
            stream = false 
        };
        
        var response = await _http.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadFromJsonAsync<JsonObject>();
        return json!["response"]!.ToString();
    }
}