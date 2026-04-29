using ModelContextProtocol.Client;

namespace Ch05.CachedMcpToolProvider;

public sealed class CachedMcpToolProvider : IDisposable
{
    private readonly McpClient _client;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly IDisposable _subscription;
    private IList<McpClientTool>? _cached;

    public CachedMcpToolProvider(McpClient client)
    {
        _client = client;
        _subscription = client.OnToolsListChanged(async _ =>
        {
            await _lock.WaitAsync();
            try { _cached = null; } finally { _lock.Release(); }
        });
    }

    public async Task<IList<McpClientTool>> GetToolsAsync(CancellationToken ct = default)
    {
        if (_cached is { } cached) return cached;

        await _lock.WaitAsync(ct);
        try
        {
            return _cached ??= await _client.ListToolsAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
        _lock.Dispose();
    }
}
