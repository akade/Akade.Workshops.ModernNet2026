using Microsoft.Extensions.AI;

namespace AI.StarterApp.TicketTriage;

internal sealed class TokenUsageCounter
{
    private long _input;
    private long _output;

    public long InputTokens => Interlocked.Read(ref _input);
    public long OutputTokens => Interlocked.Read(ref _output);

    public void Add(UsageDetails? usage)
    {
        if (usage is null)
        {
            return;
        }

        Interlocked.Add(ref _input, usage.InputTokenCount ?? 0);
        Interlocked.Add(ref _output, usage.OutputTokenCount ?? 0);
    }
}

internal sealed class TokenCountingChatClient(IChatClient innerClient, TokenUsageCounter counter) : DelegatingChatClient(innerClient)
{
    public override async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await base.GetResponseAsync(messages, options, cancellationToken);
        counter.Add(response.Usage);
        return response;
    }

    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var update in base.GetStreamingResponseAsync(messages, options, cancellationToken))
        {
            foreach (var content in update.Contents)
            {
                if (content is UsageContent usage)
                {
                    counter.Add(usage.Details);
                }
            }

            yield return update;
        }
    }
}
