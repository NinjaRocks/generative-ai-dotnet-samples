using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

const string ThreadStateFile = "thread.json";

IChatClient chat = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

AIAgent agent = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "Note",
    Instructions = "You are a personal notebook assistant. Remember anything the user tells you.",
});

AgentThread thread = LoadThread(agent) ?? agent.GetNewThread();

Console.WriteLine($"Thread file: {Path.GetFullPath(ThreadStateFile)}");
Console.WriteLine("Type messages (blank to save and exit). Re-run to resume.");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    var response = await agent.RunAsync(input, thread);
    Console.WriteLine($"[{agent.Name}] {response}");
}

await SaveThreadAsync(thread);
Console.WriteLine($"Saved {ThreadStateFile}.");

static AgentThread? LoadThread(AIAgent agent)
{
    if (!File.Exists(ThreadStateFile)) return null;
    var json = File.ReadAllText(ThreadStateFile);
    return agent.DeserializeThread(json);
}

static async Task SaveThreadAsync(AgentThread thread)
{
    var json = thread.Serialize();
    await File.WriteAllTextAsync(ThreadStateFile, json);
}
