using Anthropic.SDK;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
    ?? throw new InvalidOperationException("Set ANTHROPIC_API_KEY.");

var modelId = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL") ?? "claude-haiku-4-5-20251001";

IChatClient chat = new AnthropicClient(apiKey).Messages;

ChatClientAgent agent = new(chat, new ChatClientAgentOptions
{
    Name = "Claude",
    ChatOptions = new ChatOptions
    {
        ModelId = modelId,
        Instructions = "You are a thoughtful tutor. Explain things clearly with one concrete example.",
    },
});

AgentSession session = await agent.CreateSessionAsync();

string[] turns =
[
    "Explain how garbage collection generations work in .NET.",
    "When does an object actually move from gen 0 to gen 1?",
];

foreach (var turn in turns)
{
    Console.WriteLine($"\n[User] {turn}");
    AgentResponse response = await agent.RunAsync(turn, session);
    Console.WriteLine($"[{agent.Name}] {response}");
}
