using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chat = new OllamaApiClient(
    new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

ChatClientAgent agent = new(
    chat,
    instructions: """
        You are Curio, a curious assistant that asks one short clarifying question
        before answering. Keep replies under three sentences.
        """,
    name: "Curio");

AgentSession session = await agent.CreateSessionAsync();

string[] turns =
[
    "I'd like to plan a weekend hike.",
    "Two people, intermediate fitness, prefer woodland trails.",
];

foreach (var turn in turns)
{
    Console.WriteLine($"\n[User] {turn}");
    AgentResponse response = await agent.RunAsync(turn, session);
    Console.WriteLine($"[{agent.Name}] {response}");
}
