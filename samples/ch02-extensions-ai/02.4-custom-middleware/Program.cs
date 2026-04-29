using Ch02.CustomMiddleware;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient inner = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

IChatClient chat = new ChatClientBuilder(inner)
    .Use((next, services) => new TokenBudgetChatClient(next, dailyBudget: 200))
    .Build();

string[] prompts =
[
    "Write a 30-word product blurb for a coffee subscription service.",
    "Write a 30-word product blurb for a vinyl record subscription service.",
    "Write a 30-word product blurb for a board-game subscription service.",
];

foreach (var prompt in prompts)
{
    try
    {
        var response = await chat.GetResponseAsync(prompt);
        Console.WriteLine($"==> {response.Text}");
    }
    catch (InvalidOperationException ex)
    {
        Console.Error.WriteLine($"ABORTED: {ex.Message}");
        break;
    }
}
