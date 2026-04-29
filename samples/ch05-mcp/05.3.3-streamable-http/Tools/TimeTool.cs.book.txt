using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Ch05.StreamableHttp.Tools;

[McpServerToolType]
public sealed class TimeTool
{
    [McpServerTool, Description("Returns the current UTC time in ISO 8601 format.")]
    public static string UtcNow() => DateTime.UtcNow.ToString("O");
}
