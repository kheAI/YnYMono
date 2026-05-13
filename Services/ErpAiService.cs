using Microsoft.EntityFrameworkCore;
using Pgvector;
using YnYMono.Data;

namespace YnYMono.Services;

public class ErpAiService
{
    private readonly AppDbContext _db;
    private readonly IAiProvider _ai;

    public ErpAiService(AppDbContext db, IAiProvider ai)
    {
        _db = db;
        _ai = ai;
    }

    public async Task<string> TroubleshootMachineAsync(string productCode, string issueDescription)
    {
        // 1. Convert the user's issue into a mathematical vector
        var queryFloats = await _ai.GetEmbeddingAsync(issueDescription);
        var queryVector = new Vector(queryFloats);

        // 2. Perform Vector Search in Cloud SQL using C# LINQ (Cosine Distance <=>)
        // This is the magic part! It finds the closest matching machine manual.
        var relevantManual = await _db.ManualKnowledge
            .Where(m => m.ProductCode == productCode)
            .OrderBy(m => m.Embedding!.CosineDistance(queryVector))
            .Select(m => m.TextChunk)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(relevantManual))
            return "No manual found for this product code.";

        // 3. Construct the prompt for the LLM
        var systemPrompt = $"You are an expert ERP and Industrial AI assistant. Use this manual excerpt to answer: {relevantManual}";
        
        // 4. Generate the final answer
        return await _ai.GenerateTextAsync(systemPrompt, issueDescription);
    }
}