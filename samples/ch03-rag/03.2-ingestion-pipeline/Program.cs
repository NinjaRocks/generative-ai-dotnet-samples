using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;

const int ChunkChars = 400;
const int OverlapChars = 80;

IEmbeddingGenerator<string, Embedding<float>> embedder = new OllamaApiClient(
    new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text");

var documents = new[]
{
    new Document("welcome.md", """
        # Welcome to Contoso

        Contoso ships products globally. Customers may track their orders from the app.
        We offer free returns within 30 days for any reason.

        ## Loyalty
        Loyalty members earn points on every dollar spent. Redeem points for free shipping
        or one-off product discounts.
        """),
    new Document("warranty.md", """
        # Warranty

        Every Contoso product carries a one-year manufacturer warranty.
        Defects in materials or workmanship are repaired or replaced at no cost.
        Cosmetic damage caused by misuse is not covered.
        """),
};

var index = new List<ChunkRecord>();

foreach (var doc in documents)
{
    Console.WriteLine($"Ingesting {doc.Source}");

    var chunks = Chunk(doc.Text, ChunkChars, OverlapChars).ToList();
    Console.WriteLine($"  {chunks.Count} chunk(s)");

    var embeddings = await embedder.GenerateAsync(chunks.Select(c => c.Text).ToList());

    for (int i = 0; i < chunks.Count; i++)
    {
        index.Add(new ChunkRecord(
            Id: $"{doc.Source}#{i}",
            Source: doc.Source,
            ContentHash: Hash(chunks[i].Text),
            Text: chunks[i].Text,
            Vector: embeddings[i].Vector.ToArray()));
    }
}

Console.WriteLine();
Console.WriteLine($"Index now contains {index.Count} chunk(s):");
foreach (var c in index)
{
    Console.WriteLine($"  {c.Id} (hash {c.ContentHash[..8]}) -> {c.Text[..Math.Min(c.Text.Length, 60)].ReplaceLineEndings(" ")}...");
}

static IEnumerable<(string Text, int StartChar)> Chunk(string text, int size, int overlap)
{
    if (text.Length <= size)
    {
        yield return (text, 0);
        yield break;
    }

    int step = Math.Max(1, size - overlap);
    for (int start = 0; start < text.Length; start += step)
    {
        var slice = text.Substring(start, Math.Min(size, text.Length - start));
        yield return (slice, start);
        if (start + size >= text.Length) break;
    }
}

static string Hash(string s)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(s));
    return Convert.ToHexStringLower(bytes);
}

internal sealed record Document(string Source, string Text);
internal sealed record ChunkRecord(string Id, string Source, string ContentHash, string Text, float[] Vector);
