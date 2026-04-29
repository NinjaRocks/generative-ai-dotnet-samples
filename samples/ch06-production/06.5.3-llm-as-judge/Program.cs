using System.ComponentModel;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient produce = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();
IChatClient judge = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

(string question, string expected)[] cases =
[
    ("What's the capital of France?", "Paris"),
    ("Who wrote 'Pride and Prejudice'?", "Jane Austen"),
    ("How many planets are in our solar system?", "Eight"),
];

double total = 0;

foreach (var (question, expected) in cases)
{
    var answer = (await produce.GetResponseAsync(question)).Text ?? "";

    var verdict = await judge.GetResponseAsync<JudgeVerdict>(
    [
        new ChatMessage(ChatRole.System, """
            Score the candidate answer 1-5 on:
              - relevance: does it address the question?
              - groundedness: is it factually consistent with the expected answer?
              - coherence: is it well-formed?
            Return JSON only.
            """),
        new ChatMessage(ChatRole.User, $"Question: {question}\nExpected: {expected}\nCandidate: {answer}"),
    ]);

    if (!verdict.TryGetResult(out var v))
    {
        Console.WriteLine($"  [parse-fail] {question}");
        continue;
    }

    var avg = (v.Relevance + v.Groundedness + v.Coherence) / 3.0;
    total += avg;
    Console.WriteLine($"  rel={v.Relevance} grd={v.Groundedness} coh={v.Coherence}  avg={avg:F2}  Q: {question}");
}

Console.WriteLine();
Console.WriteLine($"Mean overall: {total / cases.Length:F2}");

internal sealed record JudgeVerdict(
    [property: Description("1-5: addresses the question.")] int Relevance,
    [property: Description("1-5: factually consistent with expected.")] int Groundedness,
    [property: Description("1-5: well-formed.")] int Coherence);
