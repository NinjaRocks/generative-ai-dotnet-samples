using Microsoft.Extensions.AI;
using OllamaSharp;

var ollama = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434");

IEmbeddingGenerator<string, Embedding<float>> embedder =
    new OllamaApiClient(ollama, Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text");

IChatClient chat = new OllamaApiClient(
    ollama, Environment.GetEnvironmentVariable("OLLAMA_CHAT_MODEL") ?? "phi4-mini");

(string id, string text)[] kb =
[
    ("policy-1", "Refund policy: customers may request a full refund within 30 days of purchase."),
    ("policy-2", "Shipping: orders placed before 2 PM are dispatched same business day."),
    ("policy-3", "Warranty: products carry a one-year manufacturer warranty against defects."),
    ("policy-4", "Cancellation: subscriptions can be cancelled any time before the next billing cycle."),
    ("policy-5", "Loyalty rewards: spend one dollar to earn one point. Points expire after 12 months."),
];

var kbVectors = await embedder.GenerateAsync(kb.Select(d => d.text).ToList());

Console.Write("\nAsk a question about our policies: ");
var question = Console.ReadLine() ?? "How do I get a refund?";

// 1. RETRIEVE
var qVec = (await embedder.GenerateAsync([question])).First();
var top = kb
    .Select((d, i) => (d.id, d.text, score: Cosine(qVec.Vector.Span, kbVectors[i].Vector.Span)))
    .OrderByDescending(x => x.score)
    .Take(3)
    .ToList();

// 2. AUGMENT
var contextBlock = string.Join("\n", top.Select(x => $"[{x.id}] {x.text}"));
var prompt = $"""
    You answer customer questions using ONLY the policies provided.
    Cite sources using [policy-id] markers. If the answer is not in the policies, say "I don't know."

    Policies:
    {contextBlock}

    Question: {question}
    """;

// 3. GENERATE
Console.WriteLine("\nAnswer:");
await foreach (var update in chat.GetStreamingResponseAsync(prompt))
{
    Console.Write(update.Text);
}
Console.WriteLine();

Console.WriteLine("\nRetrieved chunks:");
foreach (var (id, text, score) in top)
{
    Console.WriteLine($"  {score:F3}  [{id}] {text}");
}

static float Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    float dot = 0, ma = 0, mb = 0;
    for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; ma += a[i] * a[i]; mb += b[i] * b[i]; }
    return dot / (MathF.Sqrt(ma) * MathF.Sqrt(mb));
}
