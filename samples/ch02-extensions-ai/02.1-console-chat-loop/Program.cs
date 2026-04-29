using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434";
var model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini";

IChatClient chatClient = new OllamaApiClient(new Uri(endpoint), model);

const int MaxHistoryMessages = 20; // sliding window guard

List<ChatMessage> history =
[
    new(ChatRole.System, """
        You are Chef Byte, a concise recipe assistant. Reply with one recipe per turn.
        """),
];

Console.WriteLine("Chef Byte is ready. Type a message (blank line to exit).");
while (true)
{
    Console.Write("\nYou: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    history.Add(new ChatMessage(ChatRole.User, input));

    Console.Write("Bot: ");
    var fullText = "";
    await foreach (var update in chatClient.GetStreamingResponseAsync(history))
    {
        Console.Write(update.Text);
        fullText += update.Text;
    }
    Console.WriteLine();

    history.Add(new ChatMessage(ChatRole.Assistant, fullText));

    // Sliding-window trim: keep system message + last N exchanges.
    if (history.Count > MaxHistoryMessages + 1)
    {
        history.RemoveRange(1, history.Count - 1 - MaxHistoryMessages);
    }
}
