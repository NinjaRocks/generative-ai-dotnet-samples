using Microsoft.Agents.AI;
using Microsoft.Agents.AI.A2A;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IChatClient>(_ =>
    new OllamaApiClient(
        new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
        Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini"));

builder.Services.AddSingleton<AIAgent>(sp =>
    new ChatClientAgent(sp.GetRequiredService<IChatClient>(), new ChatClientAgentOptions
    {
        Name = "ResearchSpecialist",
        Instructions = "You are an A2A research specialist. Answer concisely with cited claims.",
    }));

var app = builder.Build();

app.MapA2A(app.Services.GetRequiredService<AIAgent>(), "/a2a");

app.Run();
