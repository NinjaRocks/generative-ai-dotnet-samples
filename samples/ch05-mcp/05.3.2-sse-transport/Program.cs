// SSE-only transport was retired in ModelContextProtocol 1.x.
// The 1.x SDK exposes a single HTTP transport (streamable HTTP), which is
// fully compatible with SSE-style clients while also supporting bidirectional
// streaming. This sample now mirrors 05.3.3-streamable-http; new servers
// should use that transport directly.
//
// See: Manuscript/api-update-pending.md (5.3.2 row).

using System.ComponentModel;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();


[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the supplied text back to the caller.")]
    public static string Echo(string text) => text;
}
