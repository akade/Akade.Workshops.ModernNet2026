using Microsoft.Extensions.AI;
using AI.StarterApp.Web.Services.Ingestion;
using AI.StarterApp.Web.Services;
using AI.StarterApp.Web;
using AI.StarterApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

if (builder.Configuration["AI_PROVIDER"] == "openai")
{
    builder.AddOpenAIClient("chat")
        .AddChatClient()
        .UseFunctionInvocation()
        .UseOpenTelemetry(configure: c =>
            c.EnableSensitiveData = builder.Environment.IsDevelopment());
    builder.AddOpenAIClient("embeddings")
        .AddEmbeddingGenerator();
}
else
{
    builder.AddOllamaApiClient("chat")
        .AddChatClient()
        .UseFunctionInvocation()
        .UseOpenTelemetry(configure: c =>
            c.EnableSensitiveData = builder.Environment.IsDevelopment());
    builder.AddOllamaApiClient("embeddings")
        .AddEmbeddingGenerator();
}

builder.AddQdrantClient("vectordb");
builder.Services.AddQdrantVectorStore();
builder.Services.AddQdrantCollection<Guid, IngestedChunk>(IngestedChunk.CollectionName);
builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));
// Applies robust HTTP resilience settings for all HttpClients in the Web project,
// not across the entire solution. It's aimed at supporting Ollama scenarios due
// to its self-hosted nature and potentially slow responses.
// Remove this if you want to use the global or a different HTTP resilience policy instead.
builder.Services.AddOllamaResilienceHandler();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
