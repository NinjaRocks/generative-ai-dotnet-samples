using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.AI;
using AnthropicTextContent = Anthropic.SDK.Messaging.TextContent;
using MEAITextContent = Microsoft.Extensions.AI.TextContent;

namespace Ch04.AnthropicAgent;

// Thin IChatClient adapter that calls AnthropicClient.Messages.GetClaudeMessageAsync directly,
// bypassing Anthropic.SDK's bundled ChatClientHelper. The bundled helper was compiled against
// Microsoft.Extensions.AI.Abstractions 10.3.0 and reads HostedMcpServerTool.AuthorizationToken,
// which 10.5.0 reshaped -- so going through .Messages as IChatClient throws MissingMethodException
// at runtime under the repo's central pin. See docs/verification-log.md (2026-05-02 entry) and
// the README in this folder for the full chain.
internal sealed class AnthropicChatClient : IChatClient
{
    private const int DefaultMaxTokens = 1024;

    private readonly AnthropicClient _client;
    private readonly string _defaultModel;
    private readonly int _defaultMaxTokens;

    public AnthropicChatClient(AnthropicClient client, string defaultModel, int defaultMaxTokens = DefaultMaxTokens)
    {
        _client = client;
        _defaultModel = defaultModel;
        _defaultMaxTokens = defaultMaxTokens;
    }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var (systemMessages, anthropicMessages) = Convert(messages);

        var parameters = new MessageParameters
        {
            Model = options?.ModelId ?? _defaultModel,
            Messages = anthropicMessages,
            System = systemMessages.Count > 0 ? systemMessages : null,
            MaxTokens = options?.MaxOutputTokens ?? _defaultMaxTokens,
            Temperature = options?.Temperature is float t ? (decimal)t : null,
            TopP = options?.TopP is float p ? (decimal)p : null,
            StopSequences = options?.StopSequences?.ToArray(),
            Stream = false,
        };

        MessageResponse response = await _client.Messages
            .GetClaudeMessageAsync(parameters, cancellationToken)
            .ConfigureAwait(false);

        var text = string.Concat(response.Content.OfType<AnthropicTextContent>().Select(c => c.Text));

        return new ChatResponse(new ChatMessage(ChatRole.Assistant, text))
        {
            ResponseId = response.Id,
            ModelId = response.Model,
            FinishReason = MapFinishReason(response.StopReason),
        };
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // The chapter sample never streams. Adapt non-streaming to a single update so callers
        // that opportunistically choose streaming still get a usable response.
        var response = await GetResponseAsync(messages, options, cancellationToken).ConfigureAwait(false);
        yield return new ChatResponseUpdate(ChatRole.Assistant, response.Text)
        {
            ResponseId = response.ResponseId,
            ModelId = response.ModelId,
            FinishReason = response.FinishReason,
        };
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        return serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose()
    {
        // AnthropicClient lifetime is owned by the caller.
    }

    private static (List<SystemMessage> System, List<Message> Messages) Convert(IEnumerable<ChatMessage> source)
    {
        var systemMessages = new List<SystemMessage>();
        var anthropicMessages = new List<Message>();

        foreach (var message in source)
        {
            var text = ExtractText(message);
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            if (message.Role == ChatRole.System)
            {
                systemMessages.Add(new SystemMessage(text));
                continue;
            }

            var role = message.Role == ChatRole.Assistant ? RoleType.Assistant : RoleType.User;
            anthropicMessages.Add(new Message
            {
                Role = role,
                Content = new List<ContentBase> { new AnthropicTextContent { Text = text } },
            });
        }

        return (systemMessages, anthropicMessages);
    }

    private static string ExtractText(ChatMessage message)
    {
        if (!string.IsNullOrEmpty(message.Text))
        {
            return message.Text;
        }

        return string.Concat(message.Contents.OfType<MEAITextContent>().Select(c => c.Text));
    }

    private static ChatFinishReason? MapFinishReason(string? stopReason) => stopReason switch
    {
        "end_turn" or "stop_sequence" => ChatFinishReason.Stop,
        "max_tokens" => ChatFinishReason.Length,
        "tool_use" => ChatFinishReason.ToolCalls,
        _ => null,
    };
}
