using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Ch05.StdioTransport.Tools;

[McpServerToolType]
public sealed class PingTool
{
    [McpServerTool, Description("Returns 'pong' to verify the stdio transport is wired up.")]
    public static string Ping() => "pong";
}
