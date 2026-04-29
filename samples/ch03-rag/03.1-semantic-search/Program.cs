using Microsoft.Extensions.AI;
using OllamaSharp;

IEmbeddingGenerator<string, Embedding<float>> embedder = new OllamaApiClient(
    new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text");

string[] catalog =
[
    "Wireless earbuds with active noise cancellation, 30-hour battery, sweat-resistant.",
    "Bone-conduction running headphones with open ear design.",
    "Studio over-ear headphones with planar magnetic drivers and detachable cable.",
    "Mechanical keyboard with hot-swappable switches and RGB backlight.",
    "Ergonomic vertical mouse with thumb buttons and adjustable DPI.",
    "Aluminum laptop stand with adjustable height for ergonomic typing.",
];

var docVectors = await embedder.GenerateAsync(catalog);

while (true)
{
    Console.Write("\nQuery (blank to quit): ");
    var q = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(q)) break;

    var qEmbedding = (await embedder.GenerateAsync([q])).First();

    var ranked = catalog
        .Select((text, i) => (text, score: Cosine(qEmbedding.Vector.Span, docVectors[i].Vector.Span)))
        .OrderByDescending(x => x.score)
        .Take(3);

    Console.WriteLine("Top 3 matches:");
    foreach (var (text, score) in ranked)
    {
        Console.WriteLine($"  {score:F3}  {text}");
    }
}

static float Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    float dot = 0, ma = 0, mb = 0;
    for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; ma += a[i] * a[i]; mb += b[i] * b[i]; }
    return dot / (MathF.Sqrt(ma) * MathF.Sqrt(mb));
}
