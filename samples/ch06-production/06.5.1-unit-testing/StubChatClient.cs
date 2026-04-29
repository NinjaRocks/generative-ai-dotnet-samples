using Microsoft.Extensions.AI;

namespace Ch06.UnitTesting;

/// <summary>
/// A deterministic IChatClient that returns a queued response per call,
/// recording the messages it received so tests can assert prompt construction.
/// </summary>
public sealed class StubChatClient : IChatClient
{
    private readonly Queue<string> _replies = new();

    public List<IList<ChatMessage>> Calls { get; } = [];

    public StubChatClient EnqueueReply(string text)
    {
        _replies.Enqueue(text);
        return this;
    }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Calls.Add(messages.ToList());
        var text = _replies.Count > 0 ? _replies.Dequeue() : "[stub: no reply queued]";
        return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, text)));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(messages, options, cancellationToken);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose() { }
}
