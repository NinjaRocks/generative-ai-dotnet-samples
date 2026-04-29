using System.ComponentModel;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

var openai = new OpenAIClient(apiKey);

IChatClient cheap     = openai.GetChatClient("gpt-4o-mini").AsIChatClient();
IChatClient expensive = openai.GetChatClient("gpt-4o").AsIChatClient();

string[] queries =
[
    "What is 12 + 7?",
    "Explain in 100 words why monads are sometimes called burritos.",
    "Reverse the string 'racecar'.",
    "Write a 200-word essay comparing Kant's and Mill's views on lying.",
];

foreach (var q in queries)
{
    var route = await Classify(q);
    var picked = route == "complex" ? expensive : cheap;
    var modelName = route == "complex" ? "gpt-4o" : "gpt-4o-mini";

    Console.WriteLine($"--- {q}");
    Console.WriteLine($"    routed to: {modelName}  (classification: {route})");
    var resp = await picked.GetResponseAsync(q);
    Console.WriteLine($"    => {resp.Text}");
    Console.WriteLine();
}

async Task<string> Classify(string text)
{
    var verdict = await cheap.GetResponseAsync<RouteDecision>(
    [
        new ChatMessage(ChatRole.System, """
            Classify the user query as 'simple' or 'complex'.
            'simple' = factual, arithmetic, lookup, short transformation.
            'complex' = nuanced explanation, multi-step reasoning, long-form writing.
            Return JSON only.
            """),
        new ChatMessage(ChatRole.User, text),
    ]);

    return verdict.TryGetResult(out var v) ? v.Complexity : "simple";
}

internal sealed record RouteDecision(
    [property: Description("Either 'simple' or 'complex'.")] string Complexity);
