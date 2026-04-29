using Microsoft.Extensions.AI;

namespace Ch06.UnitTesting;

/// <summary>The system-under-test. Constructs a prompt and asks the model.</summary>
public sealed class GreetingService(IChatClient chat)
{
    public async Task<string> GreetAsync(string userName, CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You greet customers in one warm sentence. No emoji."),
            new(ChatRole.User, $"Greet a returning customer named {userName}."),
        };

        var response = await chat.GetResponseAsync(messages, cancellationToken: ct);
        return response.Text ?? string.Empty;
    }
}
