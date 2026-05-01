using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace RagEvaluation;

internal sealed record QaPair(
    [property: Description("A question whose answer is contained in the source passage.")] string Question,
    [property: Description("The minimal correct answer drawn directly from the passage.")] string GroundTruth,
    [property: Description("Echo back the source chunk id so the pair stays traceable.")] string SourceChunkId);

internal static class Bootstrap
{
    public static async Task<IReadOnlyList<QaPair>> GenerateAsync(
        IChatClient generator,
        IEnumerable<(string Id, string Text)> chunks,
        CancellationToken ct = default)
    {
        var pairs = new List<QaPair>();
        foreach (var (id, text) in chunks)
        {
            var resp = await generator.GetResponseAsync<QaPair>(
                $$"""
                Read the passage and propose ONE question whose answer is contained
                in the passage. Keep the question specific and the answer concise.
                Set sourceChunkId to "{{id}}".

                Passage:
                {{text}}
                """, cancellationToken: ct);

            if (resp.TryGetResult(out var pair))
                pairs.Add(pair);
        }
        return pairs;
    }
}
