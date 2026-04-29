using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434";
var model = Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text";

IEmbeddingGenerator<string, Embedding<float>> embedder =
    new OllamaApiClient(new Uri(endpoint), model);

string[] sentences =
[
    "The cat sat on the mat.",
    "A feline rested on the rug.",
    "Quarterly revenue grew 12 percent."
];

GeneratedEmbeddings<Embedding<float>> result = await embedder.GenerateAsync(sentences);

Console.WriteLine($"Vector dimensions: {result[0].Vector.Length}");
Console.WriteLine();

for (int i = 0; i < sentences.Length; i++)
{
    for (int j = i + 1; j < sentences.Length; j++)
    {
        var sim = CosineSimilarity(result[i].Vector.Span, result[j].Vector.Span);
        Console.WriteLine($"  cos({i},{j}) = {sim:F3}   ({Trunc(sentences[i])} <-> {Trunc(sentences[j])})");
    }
}

static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
{
    float dot = 0, magA = 0, magB = 0;
    for (int i = 0; i < a.Length; i++)
    {
        dot += a[i] * b[i];
        magA += a[i] * a[i];
        magB += b[i] * b[i];
    }
    return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
}

static string Trunc(string s) => s.Length <= 30 ? s : s[..27] + "...";
