using Anthropic.SDK;
using Microsoft.Extensions.AI;

var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("ANTHROPIC_API_KEY not set.");
    return 1;
}

string[] modelsToVerify =
[
    "claude-opus-4-7",
    "claude-sonnet-4-6",
    "claude-haiku-4-5-20251001",
];

IChatClient chat = new AnthropicClient(apiKey).Messages;

var any = false;
foreach (var model in modelsToVerify)
{
    try
    {
        var response = await chat.GetResponseAsync(
            "Reply with exactly: OK",
            new ChatOptions { ModelId = model });
        Console.WriteLine($"PASS  {model}: {response.Text?.Trim()}");
    }
    catch (Exception ex)
    {
        any = true;
        Console.WriteLine($"FAIL  {model}: {ex.GetType().Name}: {ex.Message}");
    }
}

return any ? 2 : 0;
