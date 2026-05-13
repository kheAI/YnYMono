using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector; // The magic AI vector library

namespace YnYMono.Models;

public class ManualKnowledge
{
    [Key]
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string TextChunk { get; set; } = string.Empty;
    
    // This tells EF Core to create a 768-dimensional vector column
    [Column(TypeName = "vector(768)")]
    public Vector? Embedding { get; set; }
}