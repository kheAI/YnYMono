using OllamaSharp;

namespace YnYMono.Services;

public class LocalGemmaProvider : IAiProvider
{
    private readonly OllamaApiClient _ollama;

    public LocalGemmaProvider()
    {
        // Connect to local Ollama instance
        _ollama = new OllamaApiClient(new Uri("http://localhost:11434"));
        _ollama.SelectedModel = "gemma"; // Or whatever you named your gemma-4-26b-a4b-it equivalent locally
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var response = await _ollama.EmbeddingsAsync(new OllamaSharp.Models.EmbeddingsRequest {
            Model = "nomic-embed-text", // Standard fast local embedding model
            Prompt = text
        });
        return response.Embedding;
    }

    public async Task<string> GenerateTextAsync(string systemPrompt, string userPrompt)
    {
        var response = await _ollama.GenerateAsync(new OllamaSharp.Models.GenerateRequest {
            Prompt = $"{systemPrompt}\n\n{userPrompt}"
        });
        return response.Response;
    }
}