using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY (this sample uses an LLM judge).");

IChatClient judge = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

var golden = JsonSerializer.Deserialize<List<EvalCase>>(
    await File.ReadAllTextAsync("golden-dataset.json"))!;

double totalFaithfulness = 0, totalRelevance = 0;

foreach (var c in golden)
{
    // For the demo, we treat the ground-truth as the "candidate answer" so
    // the harness scores 5/5. Replace with your real RAG answer in production.
    var candidateAnswer = c.ExpectedAnswer;

    var verdict = await judge.GetResponseAsync<JudgeVerdict>(
    [
        new ChatMessage(ChatRole.System, """
            You are a strict evaluator. Score answers from 1-5 on:
              - faithfulness: is the answer supported by the ground truth?
              - relevance: does the answer actually address the question?
            Return JSON only.
            """),
        new ChatMessage(ChatRole.User, $"""
            Question: {c.Question}
            Ground truth: {c.GroundTruth}
            Candidate answer: {candidateAnswer}
            """),
    ]);

    if (!verdict.TryGetResult(out var v))
    {
        Console.WriteLine($"  {c.Id}  malformed verdict.");
        continue;
    }

    Console.WriteLine($"  {c.Id}  faithfulness={v.Faithfulness} relevance={v.Relevance}");
    totalFaithfulness += v.Faithfulness;
    totalRelevance += v.Relevance;
}

Console.WriteLine();
Console.WriteLine($"Mean faithfulness: {totalFaithfulness / golden.Count:F2}");
Console.WriteLine($"Mean relevance:    {totalRelevance / golden.Count:F2}");

internal sealed record EvalCase(string Id, string Question, string GroundTruth, string ExpectedAnswer);

internal sealed record JudgeVerdict(
    [property: Description("1-5: how well the candidate answer is supported by the ground truth.")] int Faithfulness,
    [property: Description("1-5: how well the candidate answer addresses the question.")] int Relevance);
