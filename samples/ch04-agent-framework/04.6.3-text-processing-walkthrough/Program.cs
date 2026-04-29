using Microsoft.Agents.AI.Workflows;

WorkflowBuilder builder = new();

var upper = builder.AddExecutor(Executor.From<string, string>(
    s => s.ToUpperInvariant(),
    name: "uppercase"));

var reverse = builder.AddExecutor(Executor.From<string, string>(
    s => new string(s.Reverse().ToArray()),
    name: "reverse"));

builder.SetStartExecutor(upper);
builder.AddEdge(upper, reverse);
builder.SetOutputExecutor(reverse);

Workflow workflow = builder.Build();

var result = await workflow.RunAsync("Hello, agent!");
Console.WriteLine($"Result: {result.Output}");

Console.WriteLine();
Console.WriteLine("Streaming events:");

await foreach (WorkflowEvent evt in workflow.RunStreamingAsync("Streaming demo"))
{
    switch (evt)
    {
        case WorkflowOutputEvent output:
            Console.WriteLine($"  output: {output.Data}");
            break;
        case ExecutorCompletedEvent completed:
            Console.WriteLine($"  executor done: {completed.ExecutorName}");
            break;
    }
}
