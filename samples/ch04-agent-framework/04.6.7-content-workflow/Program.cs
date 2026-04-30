using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient chat = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

ChatClientAgent researcher = new(
    chat,
    instructions: "Return 3-5 bullet points of key facts about the subject. Plain text, dash-prefixed.",
    name: "Researcher");

ChatClientAgent writer = new(
    chat,
    instructions: "Given subject + facts, write a tight 100-150 word draft. Return only the draft prose.",
    name: "Writer");

ChatClientAgent editor = new(
    chat,
    instructions: """
        Score quality 0-1. Return JSON: {"quality": <0..1>, "notes": "<one short sentence>"}.
        """,
    name: "Editor");

var research = new FunctionExecutor<Topic, ResearchedTopic>(
    "research",
    async (t, ctx, ct) =>
    {
        var resp = await researcher.RunAsync(t.Subject, cancellationToken: ct);
        return new ResearchedTopic(t.Subject, resp.ToString());
    });

var write = new FunctionExecutor<ResearchedTopic, ArticleDraft>(
    "write",
    async (r, ctx, ct) =>
    {
        var prompt = $"Subject: {r.Subject}\nFacts:\n{r.Facts}";
        var resp = await writer.RunAsync(prompt, cancellationToken: ct);
        return new ArticleDraft(r.Subject, resp.ToString());
    });

var edit = new FunctionExecutor<ArticleDraft, EditedArticle>(
    "edit",
    async (d, ctx, ct) =>
    {
        var resp = await editor.RunAsync(d.Body, cancellationToken: ct);
        var json = JsonDocument.Parse(resp.ToString()).RootElement;
        return new EditedArticle(
            d.Subject, d.Body,
            json.GetProperty("quality").GetDouble(),
            json.GetProperty("notes").GetString() ?? "");
    });

Workflow workflow = new WorkflowBuilder(research)
    .AddEdge(research, write)
    .AddEdge(write, edit)
    .WithOutputFrom(edit)
    .Build();

var topic = new Topic("the economics of small modular nuclear reactors");
await using var run = await InProcessExecution.RunAsync(workflow, topic);

EditedArticle? article = null;
foreach (var evt in run.NewEvents)
{
    if (evt is WorkflowOutputEvent output && output.Is<EditedArticle>(out var produced))
    {
        article = produced;
    }
}

if (article is null)
{
    Console.Error.WriteLine("Workflow finished without producing an EditedArticle output.");
    return;
}

Console.WriteLine($"--- {article.Subject}");
Console.WriteLine(article.Body);
Console.WriteLine();
Console.WriteLine($"Editor quality: {article.Quality:F2}");
Console.WriteLine($"Editor notes:   {article.Notes}");

internal sealed record Topic(string Subject);
internal sealed record ResearchedTopic(string Subject, string Facts);
internal sealed record ArticleDraft(string Subject, string Body);
internal sealed record EditedArticle(string Subject, string Body, double Quality, string Notes);
