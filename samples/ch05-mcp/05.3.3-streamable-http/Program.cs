using System.ComponentModel;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp("/mcp");
app.MapGet("/healthz", () => Results.Ok("ok"));

app.Run();


[McpServerToolType]
public static class TimeTool
{
    [McpServerTool, Description("Returns the server's current local time.")]
    public static string Now() => DateTime.Now.ToString("u");
}
