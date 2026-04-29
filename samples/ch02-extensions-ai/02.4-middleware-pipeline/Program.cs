using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;

using var loggerFactory = LoggerFactory.Create(b => b.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss ";
}));

IDistributedCache cache = new MemoryDistributedCache(
    Options.Create(new MemoryDistributedCacheOptions()));

IChatClient inner = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

IChatClient chat = new ChatClientBuilder(inner)
    .UseLogging(loggerFactory)
    .UseDistributedCache(cache)
    .Build();

string[] prompts =
[
    "Give me a one-sentence haiku about C# generics.",
    "Give me a one-sentence haiku about C# generics.",   // identical -> served from cache
    "Give me a one-sentence haiku about LINQ.",
];

foreach (var prompt in prompts)
{
    Console.WriteLine($"\n--- request: {prompt}");
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var resp = await chat.GetResponseAsync(prompt);
    sw.Stop();
    Console.WriteLine($"({sw.ElapsedMilliseconds} ms) {resp.Text}");
}
