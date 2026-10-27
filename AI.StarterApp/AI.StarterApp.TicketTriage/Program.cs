using System.Diagnostics;
using AI.StarterApp.TicketTriage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using OllamaSharp;
using OllamaSharp.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var tokenCounter = new TokenUsageCounter();

var useOpenAI = builder.Configuration["AI_PROVIDER"] == "openai";
var chatBuilder = useOpenAI
    ? builder.AddOpenAIClient("chat").AddChatClient()
    : builder.AddOllamaApiClient("chat").AddChatClient();

chatBuilder
       .Use(inner => new TokenCountingChatClient(inner, tokenCounter))
       .UseFunctionInvocation()
       .UseOpenTelemetry(configure: c => c.EnableSensitiveData = builder.Environment.IsDevelopment());

if (useOpenAI)
{
    builder.AddOpenAIClient("embeddings")
           .AddEmbeddingGenerator();
}
else
{
    builder.AddOllamaApiClient("embeddings")
           .AddEmbeddingGenerator();
}

builder.AddQdrantClient("vectordb");
builder.Services.AddQdrantVectorStore();
builder.Services.AddSolution<ImplementYourSolutionHere>();

using var host = builder.Build();
await host.StartAsync();

var solution = host.Services.GetRequiredService<ISolution>();

// var evaluationRecords = InVisibleDataSet.Data.Records.ToArray();
// var faqArticles = InVisibleDataSet.Data.FAQArticles.ToArray();
var evaluationRecords = VisibleDataSet.Data.Records.ToArray();
var faqArticles = VisibleDataSet.Data.FAQArticles.ToArray();

var totalScore = 0.0;
var startTimestamp = Stopwatch.GetTimestamp();

Console.WriteLine($"Starting before...");
await solution.BeforeAsync(faqArticles);
Console.WriteLine($"Finished");

int index = 0;
foreach (var expected in evaluationRecords)
{
    var actual = await solution.ProcessAsync(expected.Message);
    totalScore += TicketActionExtensions.Score(expected.Action, actual);
    Console.WriteLine($"{++index}/{evaluationRecords.Length}");
}

var elapsed = Stopwatch.GetElapsedTime(startTimestamp);
var averageScore = totalScore / evaluationRecords.Length;
Console.WriteLine($"Score: {averageScore:P1} ({evaluationRecords.Length} examples)");
Console.WriteLine($"Tokens: {tokenCounter.InputTokens} input, {tokenCounter.OutputTokens} output");
// Average token cost per 2026-10-24 for all Azure OpenAI models
// (take it with a grain of salt, as it was AI calculated and doesn't take into account caching)
double tokenCost = tokenCounter.InputTokens * 3.56 + tokenCounter.OutputTokens * 15.45;
tokenCost = tokenCost / 1_000_000;
Console.WriteLine($"Token cost: {tokenCost:C}");
Console.WriteLine($"Elapsed: {elapsed}");


await host.StopAsync();


