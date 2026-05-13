using Microsoft.EntityFrameworkCore;
using YnYMono.Models;

namespace YnYMono.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<ManualKnowledge> ManualKnowledge { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CRITICAL: Tells PostgreSQL we are using the AI Vector extension
        modelBuilder.HasPostgresExtension("vector");
        base.OnModelCreating(modelBuilder);
    }
}