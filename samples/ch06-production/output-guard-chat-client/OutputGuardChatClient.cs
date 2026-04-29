using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

namespace Ch06.OutputGuard;

public sealed partial class OutputGuardChatClient(IChatClient inner) : DelegatingChatClient(inner)
{
    [GeneratedRegex(@"!\[.*?\]\((?!https?://)(.*?)\)", RegexOptions.IgnoreCase)]
    private static partial Regex SuspiciousImageMarkdown();

    [GeneratedRegex(@"\bjavascript:", RegexOptions.IgnoreCase)]
    private static partial Regex JavaScriptScheme();

    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await base.GetResponseAsync(messages, options, cancellationToken);

        // response.Text is null when the assistant returns only tool calls or
        // structured output. Skip sanitization in that case -- there is no text to scrub.
        if (response.Text is null) return response;

        var sanitized = SuspiciousImageMarkdown().Replace(response.Text, "[blocked image]");
        sanitized = JavaScriptScheme().Replace(sanitized, "blocked-scheme:");

        return sanitized == response.Text
            ? response
            : new ChatResponse(new ChatMessage(ChatRole.Assistant, sanitized));
    }
}
