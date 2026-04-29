using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434");

IEmbeddingGenerator<string, Embedding<float>> embedder = new OllamaApiClient(
    endpoint, Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text");

IChatClient chat = new OllamaApiClient(
    endpoint, Environment.GetEnvironmentVariable("OLLAMA_CHAT_MODEL") ?? "phi4-mini");

// The "knowledge base" is just a text file.
string kbPath = args.Length > 0 ? args[0] : "knowledge.txt";
if (!File.Exists(kbPath))
{
    await File.WriteAllTextAsync(kbPath, """
        Contoso ships orders globally with same-day dispatch for orders placed before 2 PM.
        Refunds are accepted within 30 days of purchase.
        Loyalty members earn 1 point per dollar spent; points expire after 12 months.
        Every Contoso product carries a 1-year manufacturer warranty.
        Subscriptions can be cancelled any time before the next billing cycle.
        """);
    Console.WriteLine($"Created sample {kbPath}");
}

var lines = (await File.ReadAllLinesAsync(kbPath))
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .ToArray();

var lineVectors = await embedder.GenerateAsync(lines);

Console.WriteLine($"\nLocal RAG ready -- {lines.Length} chunk(s). Type a question (blank to quit).");
while (true)
{
    Console.Write("\n> ");
    var q = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(q)) break;

    var qVec = (await embedder.GenerateAsync([q])).First();
    var top = lines
        .Select((t, i) => (t, score: Cosine(qVec.Vector.Span, lineVectors[i].Vector.Span)))
        .OrderByDescending(x => x.score)
        .Take(3)
        .ToList();

    var context = string.Join("\n", top.Select((x, i) => $"[{i + 1}] {x.t}"));
    var prompt = $"Use ONLY these facts. Cite sources [n].\n\n{context}\n\nQ: {q}";

    await foreach (var update in chat.GetStreamingResponseAsync(prompt))
    {
        Console.Write(update.Text);
    }
    Console.WriteLine();
}

static float Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    float dot = 0, ma = 0, mb = 0;
    for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; ma += a[i] * a[i]; mb += b[i] * b[i]; }
    return dot / (MathF.Sqrt(ma) * MathF.Sqrt(mb));
}
