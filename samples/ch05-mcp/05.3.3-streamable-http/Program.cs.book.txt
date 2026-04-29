using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpServerTransport(opts => opts.TransportType = HttpTransportType.StreamableHttp)
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp("/mcp");
app.MapGet("/healthz", () => Results.Ok("ok"));

app.Run();
