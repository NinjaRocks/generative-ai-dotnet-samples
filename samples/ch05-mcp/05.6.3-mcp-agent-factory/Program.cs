using Ch05.McpAgentFactory;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;

var builder = Host.CreateApplicationBuilder(args);

var apiKey = builder.Configuration["OpenAI:ApiKey"]
    ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException(
        "Set OpenAI:ApiKey in configuration or OPENAI_API_KEY environment variable.");

var modelId = builder.Configuration["OpenAI:Model"] ?? "gpt-4o-mini";

builder.Services.AddSingleton(new OpenAIClient(apiKey));

builder.Services.AddChatClient(sp =>
    sp.GetRequiredService<OpenAIClient>()
        .GetChatClient(modelId)
        .AsIChatClient())
    .UseFunctionInvocation()
    .UseLogging();

builder.Services.AddSingleton<McpAgentFactory>();

var host = builder.Build();

await using var factory = host.Services.GetRequiredService<McpAgentFactory>();
var opts = builder.Configuration.GetSection("Agents:TravelPlanner").Get<AgentDefinitionOptions>()
    ?? throw new InvalidOperationException("Missing Agents:TravelPlanner configuration.");

AIAgent agent = await factory.CreateAsync(opts);
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine($"Agent '{agent.Name}' ready. Type a message (blank line to exit).");

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    AgentResponse response = await agent.RunAsync(input, session);
    Console.WriteLine(response);
}
