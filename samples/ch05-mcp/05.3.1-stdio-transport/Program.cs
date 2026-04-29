using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// stderr is the only log stream that's safe to write to over stdio --
// stdout is reserved for JSON-RPC protocol traffic.
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.LogToStandardErrorThreshold = LogLevel.Trace;
    o.SingleLine = true;
});

await builder.Build().RunAsync();
