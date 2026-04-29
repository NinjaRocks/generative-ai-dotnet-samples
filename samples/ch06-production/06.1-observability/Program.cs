using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string ServiceName = "ch06.observability.demo";

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var inner = new OllamaApiClient(
        new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
        Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

    return new ChatClientBuilder(inner)
        .UseOpenTelemetry(sourceName: ServiceName, configure: o => o.EnableSensitiveData = false)
        .Build();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(ServiceName))
    .WithTracing(t => t
        .AddSource(ServiceName)
        .AddSource("Microsoft.Extensions.AI")
        .AddOtlpExporter()); // honors OTEL_EXPORTER_OTLP_ENDPOINT (default http://localhost:4317)

using var host = builder.Build();
await host.StartAsync();

var chat = host.Services.GetRequiredService<IChatClient>();

await chat.GetResponseAsync("Give me a one-line C# tip.");
await chat.GetResponseAsync("Give me a one-line LINQ tip.");

Console.WriteLine();
Console.WriteLine("Two requests sent. Spans printed above (and exported to OTLP if configured).");
Console.WriteLine($"Service: {ServiceName}.   To send to a collector:  export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317");

await host.StopAsync();
