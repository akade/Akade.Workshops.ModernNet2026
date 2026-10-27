var builder = DistributedApplication.CreateBuilder(args);

bool useGpu = false;
bool startChatApp = true;
// true: use an external OpenAI-compatible endpoint (chat + embeddings) instead of a local Ollama container
bool useOpenAICompatible = false;

IResourceBuilder<IResourceWithConnectionString> chat;
IResourceBuilder<IResourceWithConnectionString> embeddings;

if (useOpenAICompatible)
{
    // Unset secret parameters are prompted for in the Aspire dashboard
    var apiKey = builder.AddParameter("openai-apikey", secret: true);
    var openai = builder.AddOpenAI("lyceum")
        .WithApiKey(apiKey)
        .WithEndpoint("https://api.lyceum.technology/openai/v1/");

    chat = openai.AddModel("chat", "z-ai/glm-5.3-flash");
    embeddings = openai.AddModel("embeddings", "qwen/qwen3-embedding-8b");
}
else
{
    var ollama = builder.AddOllama("ollama")
        .WithImageTag("0.40.6")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume("ollama");

    if (useGpu)
    {
        ollama.WithGPUSupport();
    }

    // Bonsai model family
    // -------------------
    // bonsai-2:2b  - 911 MB
    // bonsai-2:4b  - 1.6 GB
    // bonsai-2:9b  - 3.2 GB
    // bonsai-2:27b - 8.3 GB
    chat = ollama.AddModel("chat", "bonsai-2:2b");
    embeddings = ollama.AddModel("embeddings", "all-minilm");
}

var vectorDB = builder.AddQdrant("vectordb")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var markitdown = builder.AddContainer("markitdown", "mcp/markitdown")
    .WithArgs("--http", "--host", "0.0.0.0", "--port", "3001")
    .WithHttpEndpoint(targetPort: 3001, name: "http");

if (startChatApp)
{
    var webApp = builder.AddProject<Projects.AI_StarterApp_Web>("aichatweb-app");
    webApp
        .WithReference(chat)
        .WithReference(embeddings)
        .WithEnvironment("AI_PROVIDER", useOpenAICompatible ? "openai" : "ollama")
        .WaitFor(chat)
        .WaitFor(embeddings);
    webApp
        .WithReference(vectorDB)
        .WaitFor(vectorDB);
    webApp
        .WithEnvironment("MARKITDOWN_MCP_URL", markitdown.GetEndpoint("http"));
}

var ticketTriage = builder.AddProject<Projects.AI_StarterApp_TicketTriage>("ai-starterapp-tickettriage");
ticketTriage
    .WithReference(chat)
    .WithEnvironment("AI_PROVIDER", useOpenAICompatible ? "openai" : "ollama")
    .WaitFor(chat)
    .WithReference(embeddings)
    .WaitFor(embeddings)
    .WithReference(vectorDB)
    .WaitFor(vectorDB)
    .WithExplicitStart();

builder.Build().Run();
