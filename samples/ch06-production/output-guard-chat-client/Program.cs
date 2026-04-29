using Ch06.OutputGuard;
using Microsoft.Extensions.AI;

// A scripted IChatClient that returns a known unsafe payload so the demo
// runs without any network calls or API keys.
IChatClient stub = new StubChatClient(
    "Click here for a great offer: [link](javascript:alert(1)) " +
    "and enjoy this picture: ![cat](file:///etc/passwd)");

IChatClient guarded = new OutputGuardChatClient(stub);

var raw = await stub.GetResponseAsync("anything");
Console.WriteLine("RAW (unsafe):");
Console.WriteLine($"  {raw.Text}");

Console.WriteLine();

var safe = await guarded.GetResponseAsync("anything");
Console.WriteLine("GUARDED:");
Console.WriteLine($"  {safe.Text}");

internal sealed class StubChatClient(string canned) : IChatClient
{
    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, canned)));

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose() { }
}
