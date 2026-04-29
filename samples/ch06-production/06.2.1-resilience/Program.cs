using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = Host.CreateApplicationBuilder(args);

// A flaky test endpoint: half the requests return 503.
int callCount = 0;

builder.Services.AddHttpClient("flaky-ai", c => c.BaseAddress = new Uri("https://example.invalid/"))
    .AddStandardResilienceHandler(o =>
    {
        o.Retry.MaxRetryAttempts = 3;
        o.Retry.Delay = TimeSpan.FromMilliseconds(200);
        o.Retry.UseJitter = true;
        o.Retry.BackoffType = DelayBackoffType.Exponential;
        o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
        o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
        o.CircuitBreaker.FailureRatio = 0.5;
        o.CircuitBreaker.MinimumThroughput = 4;
        o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler(() => new SimulatedFlakyHandler(() => Interlocked.Increment(ref callCount)));

using var host = builder.Build();
await host.StartAsync();

var factory = host.Services.GetRequiredService<IHttpClientFactory>();
var http = factory.CreateClient("flaky-ai");

try
{
    Console.WriteLine("Sending request through the standard resilience pipeline...");
    var response = await http.GetAsync("/v1/something");
    Console.WriteLine($"Status: {response.StatusCode} (after {callCount} HTTP attempt(s))");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed after {callCount} attempt(s): {ex.GetType().Name} -- {ex.Message}");
}

await host.StopAsync();

internal sealed class SimulatedFlakyHandler(Action onAttempt) : DelegatingHandler
{
    private int _i;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        onAttempt();
        var attempt = Interlocked.Increment(ref _i);
        var status = attempt < 3 ? HttpStatusCode.ServiceUnavailable : HttpStatusCode.OK;
        return Task.FromResult(new HttpResponseMessage(status));
    }
}
