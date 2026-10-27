using System.Collections.Immutable;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using OllamaSharp;
using OllamaSharp.Models;

namespace AI.StarterApp.TicketTriage;

/// <summary>
/// Starting point for your own solution. Currently returns random results.
/// </summary>
internal sealed class ImplementYourSolutionHere(
    IChatClient chatClient,
    VectorStoreCollection<ulong, FAQRecord> faqCollection) : ISolution, ISolutionRegistration
{
    // Pass to chatClient.GetResponseAsync<TicketAction>(prompt, _chatOptions) to disable thinking in Ollama
    private readonly ChatOptions _chatOptions = new ChatOptions().AddOllamaOption(OllamaOption.Think, false);

    private FAQArticleId[] _articleIds = [];

    public static void RegisterServices(IServiceCollection services)
    {
        services.AddQdrantCollection<ulong, FAQRecord>(FAQRecord.CollectionName);
    }

    public async Task BeforeAsync(FAQArticle[] faqArticles)
    {
        await faqCollection.EnsureCollectionDeletedAsync();
        await faqCollection.EnsureCollectionExistsAsync();

        _articleIds = [.. faqArticles.Select(a => a.Id)];
    }

    public Task<TicketAction> ProcessAsync(IncomingMessage message)
    {
        TicketAction result = Random.Shared.Next(3) switch
        {
            0 when _articleIds.Length > 0 => new TicketAction(new FAQ([Random.Shared.GetItems(_articleIds, 1)[0]])),
            1 => new TicketAction(new Duplicate(Guid.NewGuid())),
            _ => new TicketAction(new NewTicket(
                Random.Shared.GetItems(Enum.GetValues<Category>(), 1)[0],
                Random.Shared.GetItems(Enum.GetValues<Kind>(), 1)[0],
                ImmutableHashSet<AffectedDevices>.Empty))
        };

        return Task.FromResult(result);
    }
}
