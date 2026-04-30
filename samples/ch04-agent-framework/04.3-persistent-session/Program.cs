using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

const string SessionStateFile = "session.json";

IChatClient chat = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

ChatClientAgent agent = new(
    chat,
    instructions: "You are a personal notebook assistant. Remember anything the user tells you.",
    name: "Note");

AgentSession session = await LoadSessionAsync(agent) ?? await agent.CreateSessionAsync();

Console.WriteLine($"Session file: {Path.GetFullPath(SessionStateFile)}");
Console.WriteLine("Type messages (blank to save and exit). Re-run to resume.");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    AgentResponse response = await agent.RunAsync(input, session);
    Console.WriteLine($"[{agent.Name}] {response}");
}

await SaveSessionAsync(agent, session);
Console.WriteLine($"Saved {SessionStateFile}.");

static async Task<AgentSession?> LoadSessionAsync(AIAgent agent)
{
    if (!File.Exists(SessionStateFile)) return null;
    using var stream = File.OpenRead(SessionStateFile);
    var element = await JsonSerializer.DeserializeAsync<JsonElement>(stream);
    return await agent.DeserializeSessionAsync(element);
}

static async Task SaveSessionAsync(AIAgent agent, AgentSession session)
{
    JsonElement element = await agent.SerializeSessionAsync(session);
    await File.WriteAllTextAsync(SessionStateFile, element.GetRawText());
}
