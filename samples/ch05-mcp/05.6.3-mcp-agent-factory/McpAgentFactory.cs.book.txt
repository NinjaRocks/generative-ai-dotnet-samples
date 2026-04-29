using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;

namespace Ch05.McpAgentFactory;

public sealed class McpServerOptions
{
    public required string Transport { get; init; }
    public string? Command { get; init; }
    public IList<string>? Args { get; init; }
    public string? Url { get; init; }
    public IDictionary<string, string?>? EnvironmentVariables { get; init; }
}

public sealed class AgentDefinitionOptions
{
    public required string Name { get; init; }
    public required string Instructions { get; init; }
    public IList<McpServerOptions> McpServers { get; init; } = new List<McpServerOptions>();
}

public sealed class McpAgentFactory(
    IChatClient chat,
    ILogger<McpAgentFactory> logger) : IAsyncDisposable
{
    private readonly List<McpClient> _clients = new();

    public async Task<AIAgent> CreateAsync(AgentDefinitionOptions opts, CancellationToken ct = default)
    {
        var tools = new List<AIFunction>();

        foreach (var server in opts.McpServers)
        {
            try
            {
                IClientTransport transport = BuildTransport(server);
                McpClient client = await McpClient.CreateAsync(transport, cancellationToken: ct);
                _clients.Add(client);

                IList<McpClientTool> serverTools = await client.ListToolsAsync(ct);
                foreach (var t in serverTools)
                {
                    var prefix = SanitizeName(server.Command ?? server.Url ?? "mcp");
                    tools.Add(t.AsAIFunction(new AIFunctionFactoryOptions
                    {
                        Name = $"{prefix}_{t.Name}"
                    }));
                }

                logger.LogInformation(
                    "Connected to MCP server '{Server}'. Tools loaded: {Count}.",
                    server.Url ?? server.Command, serverTools.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to connect to MCP server '{Server}'.",
                    server.Url ?? server.Command);
                throw;
            }
        }

        return new ChatClientAgent(chat, new ChatClientAgentOptions
        {
            Name = opts.Name,
            Instructions = opts.Instructions,
            Tools = tools.Cast<AITool>().ToList()
        });
    }

    private static IClientTransport BuildTransport(McpServerOptions opts) =>
        opts.Transport.ToLowerInvariant() switch
        {
            "stdio" => new StdioClientTransport(new StdioClientTransportOptions
            {
                Command = opts.Command!,
                Arguments = opts.Args?.ToArray() ?? [],
                EnvironmentVariables = opts.EnvironmentVariables
            }),
            "http" => new StreamableHttpClientTransport(new StreamableHttpClientTransportOptions
            {
                Endpoint = new Uri(opts.Url!)
            }),
            "sse" => new SseClientTransport(new SseClientTransportOptions
            {
                Endpoint = new Uri(opts.Url!)
            }),
            _ => throw new InvalidOperationException($"Unknown transport: {opts.Transport}")
        };

    private static string SanitizeName(string raw) =>
        new string(raw.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    public async ValueTask DisposeAsync()
    {
        foreach (var c in _clients)
        {
            await c.DisposeAsync();
        }
    }
}
