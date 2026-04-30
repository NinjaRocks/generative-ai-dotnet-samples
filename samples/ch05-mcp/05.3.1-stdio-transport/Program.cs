using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// stderr is the only log stream that's safe to write to over stdio --
// stdout is reserved for JSON-RPC protocol traffic.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Logging.AddSimpleConsole(o => o.SingleLine = true);

await builder.Build().RunAsync();


[McpServerToolType]
public static class TimeTool
{
    [McpServerTool, Description("Returns the server's current local time.")]
    public static string Now() => DateTime.Now.ToString("u");
}
