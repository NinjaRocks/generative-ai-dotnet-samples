using Ch05.CachedMcpToolProvider;
using ModelContextProtocol.Client;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- <mcp-server-command> [args...]");
    Console.Error.WriteLine("Example: dotnet run -- dnx Microsoft.Azure.Mcp --yes");
    return 1;
}

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Command = args[0],
    Arguments = args.Skip(1).ToArray()
});

await using var client = await McpClient.CreateAsync(transport);
using var provider = new CachedMcpToolProvider(client);

Console.WriteLine("First call -- fetches from server.");
var firstFetch = await provider.GetToolsAsync();
Console.WriteLine($"  {firstFetch.Count} tool(s):");
foreach (var t in firstFetch)
{
    Console.WriteLine($"    - {t.Name}");
}

Console.WriteLine();
Console.WriteLine("Second call -- served from cache.");
var secondFetch = await provider.GetToolsAsync();
Console.WriteLine($"  {secondFetch.Count} tool(s) (cached={ReferenceEquals(firstFetch, secondFetch)})");

return 0;
