using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Ch05.CachedMcpToolProvider;

public sealed class CachedMcpToolProvider : IAsyncDisposable
{
    private readonly McpClient _client;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IAsyncDisposable _subscription;
    private IList<McpClientTool>? _cached;

    public CachedMcpToolProvider(McpClient client)
    {
        _client = client;
        _subscription = _client.RegisterNotificationHandler(
            NotificationMethods.ToolListChangedNotification,
            async (_, _) =>
            {
                await _lock.WaitAsync().ConfigureAwait(false);
                try { _cached = null; } finally { _lock.Release(); }
            });
    }

    public async Task<IList<McpClientTool>> GetToolsAsync(CancellationToken ct = default)
    {
        if (_cached is { } cached) return cached;

        await _lock.WaitAsync(ct);
        try
        {
            return _cached ??= await _client.ListToolsAsync(cancellationToken: ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _subscription.DisposeAsync().ConfigureAwait(false);
        _lock.Dispose();
    }
}
