using Microsoft.EntityFrameworkCore;
using YnYMono.Data;
using YnYMono.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient(); // For Cloud Gemini

// Configure Cloud SQL Database with pgvector
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector() // Enable Vector operations
    ).UseSnakeCaseNamingConvention());

// Dynamic AI Injection (Toggle based on appsettings)
bool useLocalModel = builder.Configuration.GetValue<bool>("AiSettings:UseLocalModel");
if (useLocalModel)
{
    builder.Services.AddScoped<IAiProvider, LocalGemmaProvider>();
}
else
{
    builder.Services.AddScoped<IAiProvider, CloudGeminiProvider>();
}

// Register our RAG Service
builder.Services.AddScoped<ErpAiService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<YnYMono.Components.App>()
   .AddInteractiveServerRenderMode();

app.Run();