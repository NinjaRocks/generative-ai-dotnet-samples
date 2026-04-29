using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Ch05.SseTransport.Tools;

[McpServerToolType]
public sealed class EchoTool
{
    [McpServerTool, Description("Returns the input message back to the caller. Used to verify the SSE transport is wired up correctly.")]
    public static string Echo(
        [Description("The text to echo back.")] string message)
        => $"Echo: {message}";
}
