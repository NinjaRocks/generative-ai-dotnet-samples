using Microsoft.Extensions.AI;
using Xunit;

namespace Ch06.UnitTesting;

public sealed class GreetingServiceTests
{
    [Fact]
    public async Task Sends_system_then_user_message_with_user_name()
    {
        var stub = new StubChatClient().EnqueueReply("Welcome back, Anna!");
        var sut = new GreetingService(stub);

        var greeting = await sut.GreetAsync("Anna");

        Assert.Equal("Welcome back, Anna!", greeting);
        Assert.Single(stub.Calls);

        var sent = stub.Calls[0];
        Assert.Equal(2, sent.Count);
        Assert.Equal(ChatRole.System, sent[0].Role);
        Assert.Equal(ChatRole.User, sent[1].Role);
        Assert.Contains("Anna", sent[1].Text);
    }

    [Fact]
    public async Task Returns_empty_string_when_model_returns_no_text()
    {
        var stub = new StubChatClient(); // no queued reply -> default sentinel
        var sut = new GreetingService(stub);

        var greeting = await sut.GreetAsync("Anna");

        Assert.NotNull(greeting);
    }
}
