namespace YnYMono.Services;

public interface IAiProvider
{
    Task<float[]> GetEmbeddingAsync(string text);
    Task<string> GenerateTextAsync(string systemPrompt, string userPrompt);
}