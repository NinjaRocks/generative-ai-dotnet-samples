using Microsoft.Extensions.AI;

namespace Ch02.CustomMiddleware;

public sealed class TokenBudgetChatClient(IChatClient inner, int dailyBudget) : DelegatingChatClient(inner)
{
    private readonly Lock _gate = new();
    private DateOnly _windowStart = DateOnly.FromDateTime(DateTime.UtcNow);
    private long _consumed;

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        EnsureBudget();
        var response = await base.GetResponseAsync(messages, options, cancellationToken);
        Account(response.Usage);
        return response;
    }

    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        EnsureBudget();
        await foreach (var update in base.GetStreamingResponseAsync(messages, options, cancellationToken))
        {
            yield return update;
        }
    }

    private void EnsureBudget()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        lock (_gate)
        {
            if (today != _windowStart)
            {
                _windowStart = today;
                _consumed = 0;
            }
            if (_consumed >= dailyBudget)
            {
                throw new InvalidOperationException(
                    $"Daily token budget of {dailyBudget} exceeded ({_consumed} consumed).");
            }
        }
    }

    private void Account(UsageDetails? usage)
    {
        if (usage?.TotalTokenCount is not long total) return;
        lock (_gate)
        {
            _consumed += total;
            Console.WriteLine($"  [budget] consumed {total} tokens this turn ({_consumed}/{dailyBudget} today)");
        }
    }
}
