using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- <mcp-server-command> [server-args...]");
    Console.Error.WriteLine("Example: dotnet run -- dnx Microsoft.Azure.Mcp --yes");
    return 1;
}

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Command = args[0],
    Arguments = args.Skip(1).ToArray(),
});

await using var mcp = await McpClient.CreateAsync(transport);
var mcpTools = (await mcp.ListToolsAsync()).Select(t => t.AsAIFunction()).Cast<AITool>().ToList();

// Mix one local C# tool with all the MCP-sourced tools.
var localTools = new List<AITool>
{
    AIFunctionFactory.Create(GetCurrentLocalTime),
};

IChatClient chat = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

AIAgent agent = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "MixedToolsAgent",
    Instructions = "You can use both local helper tools and MCP-sourced tools.",
    Tools = [.. localTools, .. mcpTools],
});

Console.WriteLine($"Loaded {mcpTools.Count} MCP tool(s) + {localTools.Count} local tool(s).");
Console.WriteLine("Ask the agent something. Blank line exits.\n");

var thread = agent.GetNewThread();
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;
    var response = await agent.RunAsync(input, thread);
    Console.WriteLine($"[{agent.Name}] {response}\n");
}

return 0;


[Description("Get the current local time on this machine.")]
static string GetCurrentLocalTime() => DateTime.Now.ToString("u");
