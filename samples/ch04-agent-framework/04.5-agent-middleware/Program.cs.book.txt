using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chat = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

AIAgent inner = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "Helper",
    Instructions = "Reply with a single concise sentence.",
});

AIAgent agent = inner
    .AsBuilder()
    .Use(LoggingRunMiddleware)
    .Build();

var thread = agent.GetNewThread();
var response = await agent.RunAsync("Give me a one-sentence travel tip for visiting Iceland in winter.", thread);
Console.WriteLine($"\n[{agent.Name}] {response}");


static async Task<AgentRunResponse> LoggingRunMiddleware(
    AIAgent agent,
    IList<ChatMessage> messages,
    AgentThread? thread,
    AgentRunOptions? options,
    Func<IList<ChatMessage>, AgentThread?, AgentRunOptions?, CancellationToken, Task<AgentRunResponse>> next,
    CancellationToken ct)
{
    var sw = Stopwatch.StartNew();
    Console.WriteLine($"  [run] start: \"{messages.Last().Text}\"");
    var response = await next(messages, thread, options, ct);
    sw.Stop();
    Console.WriteLine($"  [run] done in {sw.ElapsedMilliseconds} ms: {response.Messages.Count} message(s)");
    return response;
}
