using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient chat = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

AIAgent researcher = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "Researcher",
    Instructions = "Return 3-5 bullet points of key facts about the subject. Plain text, dash-prefixed.",
});

AIAgent writer = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "Writer",
    Instructions = "Given subject + facts, write a tight 100-150 word draft. Return only the draft prose.",
});

AIAgent editor = new ChatClientAgent(chat, new ChatClientAgentOptions
{
    Name = "Editor",
    Instructions = """
        Score quality 0-1. Return JSON: {"quality": <0..1>, "notes": "<one short sentence>"}.
        """,
});

var builder = new WorkflowBuilder();

var researchExec = builder.AddExecutor(Executor.FromAsync<Topic, ResearchedTopic>(
    async (t, ct) =>
    {
        var resp = await researcher.RunAsync(t.Subject, cancellationToken: ct);
        return new ResearchedTopic(t.Subject, resp.ToString());
    }, name: "research"));

var writeExec = builder.AddExecutor(Executor.FromAsync<ResearchedTopic, ArticleDraft>(
    async (r, ct) =>
    {
        var prompt = $"Subject: {r.Subject}\nFacts:\n{r.Facts}";
        var resp = await writer.RunAsync(prompt, cancellationToken: ct);
        return new ArticleDraft(r.Subject, resp.ToString());
    }, name: "write"));

var editExec = builder.AddExecutor(Executor.FromAsync<ArticleDraft, EditedArticle>(
    async (d, ct) =>
    {
        var resp = await editor.RunAsync(d.Body, cancellationToken: ct);
        var json = JsonDocument.Parse(resp.ToString()).RootElement;
        return new EditedArticle(
            d.Subject, d.Body,
            json.GetProperty("quality").GetDouble(),
            json.GetProperty("notes").GetString() ?? "");
    }, name: "edit"));

builder.SetStartExecutor(researchExec);
builder.AddEdge(researchExec, writeExec);
builder.AddEdge(writeExec, editExec);
builder.SetOutputExecutor(editExec);

Workflow workflow = builder.Build();

var topic = new Topic("the economics of small modular nuclear reactors");
var result = await workflow.RunAsync(topic);
var article = result.Output;

Console.WriteLine($"--- {article.Subject}");
Console.WriteLine(article.Body);
Console.WriteLine();
Console.WriteLine($"Editor quality: {article.Quality:F2}");
Console.WriteLine($"Editor notes:   {article.Notes}");

internal sealed record Topic(string Subject);
internal sealed record ResearchedTopic(string Subject, string Facts);
internal sealed record ArticleDraft(string Subject, string Body);
internal sealed record EditedArticle(string Subject, string Body, double Quality, string Notes);
