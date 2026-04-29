using System.Text.Json;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IChatClient>(_ =>
    new OllamaApiClient(
        new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
        Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini"));

var app = builder.Build();

app.MapGet("/", () => Results.Content(IndexHtml, "text/html"));

app.MapPost("/api/chat/stream", async (ChatRequest req, IChatClient chat, HttpResponse res, CancellationToken ct) =>
{
    res.Headers.ContentType = "text/event-stream";
    res.Headers.CacheControl = "no-cache";

    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a helpful assistant."),
        new(ChatRole.User, req.Prompt),
    };

    await foreach (var update in chat.GetStreamingResponseAsync(messages, cancellationToken: ct))
    {
        if (string.IsNullOrEmpty(update.Text)) continue;
        var payload = JsonSerializer.Serialize(new { text = update.Text });
        await res.WriteAsync($"data: {payload}\n\n", ct);
        await res.Body.FlushAsync(ct);
    }

    await res.WriteAsync("data: [DONE]\n\n", ct);
});

app.Run();

internal sealed record ChatRequest(string Prompt);

internal static class IndexConstants { }

internal partial class Program
{
    private const string IndexHtml = """
        <!doctype html>
        <html><body>
        <h1>Streaming demo</h1>
        <textarea id="p" rows="3" cols="60">Tell me a 3-sentence story.</textarea><br>
        <button onclick="go()">Send</button>
        <pre id="out"></pre>
        <script>
        async function go() {
          const out = document.getElementById('out');
          out.textContent = '';
          const res = await fetch('/api/chat/stream', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ prompt: document.getElementById('p').value })
          });
          const reader = res.body.getReader();
          const decoder = new TextDecoder();
          let buf = '';
          while (true) {
            const { done, value } = await reader.read();
            if (done) break;
            buf += decoder.decode(value, { stream: true });
            for (const line of buf.split('\n')) {
              if (!line.startsWith('data: ')) continue;
              const data = line.slice(6);
              if (data === '[DONE]') return;
              try { out.textContent += JSON.parse(data).text; } catch {}
            }
            buf = buf.split('\n').pop();
          }
        }
        </script>
        </body></html>
        """;
}
