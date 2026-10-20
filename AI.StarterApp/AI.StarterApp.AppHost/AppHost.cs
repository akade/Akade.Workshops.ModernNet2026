var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithGPUSupport()
    .WithImageTag("0.40.6")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithContainerName("ollama")
    .WithDataVolume("ollama");
// Bonsai model family
// -------------------
// bonsai-2:2b  - 911 MB
// bonsai-2:4b  - 1.6 GB
// bonsai-2:9b  - 3.2 GB
// bonsai-2:27b - 8.3 GB
var chat = ollama.AddModel("chat", "bonsai-2:4b");
var embeddings = ollama.AddModel("embeddings", "all-minilm");

var vectorDB = builder.AddQdrant("vectordb")
    .WithContainerName("vectordb")
    .WithDataVolume("vectordb")
    .WithLifetime(ContainerLifetime.Persistent);

var markitdown = builder.AddContainer("markitdown", "mcp/markitdown")
    .WithArgs("--http", "--host", "0.0.0.0", "--port", "3001")
    .WithHttpEndpoint(targetPort: 3001, name: "http");

var webApp = builder.AddProject<Projects.AI_StarterApp_Web>("aichatweb-app");
webApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
webApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);
webApp
    .WithEnvironment("MARKITDOWN_MCP_URL", markitdown.GetEndpoint("http"));

builder.Build().Run();
