using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using OpenAI;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

IChatClient chatClient = ChooseClient(config, args.FirstOrDefault());

List<ChatMessage> history =
[
    new(ChatRole.System, """
        You are Chef Byte, a friendly recipe assistant. Suggest one recipe per message.
        Include the name, a one-line description, key ingredients, and total time.
        Politely decline non-food topics.
        """),
    new(ChatRole.User, "I have chicken, garlic, and lemon. What can I make?")
];

ChatResponse response = await chatClient.GetResponseAsync(history);
Console.WriteLine($"[Assistant]: {response.Text}");

history.AddMessages(response);
history.Add(new ChatMessage(ChatRole.User, "Can I use thighs instead of breast?"));

Console.WriteLine();
Console.Write("[Assistant streaming]: ");
await foreach (var update in chatClient.GetStreamingResponseAsync(history))
{
    Console.Write(update.Text);
}
Console.WriteLine();

static IChatClient ChooseClient(IConfiguration config, string? profile)
{
    profile ??= config["Provider"] ?? "ollama";

    return profile.ToLowerInvariant() switch
    {
        "openai" => new OpenAIClient(
                config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                    ?? throw new InvalidOperationException("Set OpenAI:ApiKey or OPENAI_API_KEY."))
            .GetChatClient(config["OpenAI:Model"] ?? "gpt-4o-mini")
            .AsIChatClient(),

        "github" => new OpenAIClient(
                new System.ClientModel.ApiKeyCredential(
                    config["GitHub:Token"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN")
                    ?? throw new InvalidOperationException("Set GitHub:Token or GITHUB_TOKEN.")),
                new OpenAIClientOptions { Endpoint = new Uri("https://models.inference.ai.azure.com") })
            .GetChatClient(config["GitHub:Model"] ?? "gpt-4o-mini")
            .AsIChatClient(),

        _ => new OllamaApiClient(
            new Uri(config["Ollama:Endpoint"] ?? "http://localhost:11434"),
            config["Ollama:Model"] ?? "phi4-mini"),
    };
}
