using ProjectLoop.Engagement.Core;
using ProjectLoop.Engagement.Functions;
using Xunit;

namespace ProjectLoop.Engagement.Functions.Tests;

public class EngagementOutboxRelayTriggerTests
{
    private sealed class FakeOutboxRelay : IOutboxRelay
    {
        public int CallCount { get; private set; }

        public Task RelayAsync(CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task RunAsync_Delegates_To_The_Injected_Relay_Exactly_Once()
    {
        var relay = new FakeOutboxRelay();
        var trigger = new EngagementOutboxRelayTrigger(relay);

        await trigger.RunAsync(timer: null!, cancellationToken: default);

        Assert.Equal(1, relay.CallCount);
    }
}
