using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chat = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

ChatClientAgent inner = new(
    chat,
    instructions: "Reply with a single concise sentence.",
    name: "Helper");

AIAgent agent = inner
    .AsBuilder()
    .Use(LoggingRunMiddleware, runStreamingFunc: null)
    .Build();

AgentSession session = await inner.CreateSessionAsync();
AgentResponse response = await agent.RunAsync("Give me a one-sentence travel tip for visiting Iceland in winter.", session);
Console.WriteLine($"\n[{agent.Name}] {response}");


static async Task<AgentResponse> LoggingRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentSession? session,
    AgentRunOptions? options,
    AIAgent next,
    CancellationToken ct)
{
    var sw = Stopwatch.StartNew();
    var lastUserMessage = messages.LastOrDefault();
    Console.WriteLine($"  [run] start: \"{lastUserMessage?.Text}\"");
    AgentResponse response = await next.RunAsync(messages, session, options, ct);
    sw.Stop();
    Console.WriteLine($"  [run] done in {sw.ElapsedMilliseconds} ms: {response.Messages.Count} message(s)");
    return response;
}
