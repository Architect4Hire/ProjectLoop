using ProjectLoop.Identity.Core;
using ProjectLoop.Identity.Functions;
using Xunit;

namespace ProjectLoop.Identity.Functions.Tests;

public class IdentityOutboxRelayTriggerTests
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
        var trigger = new IdentityOutboxRelayTrigger(relay);

        await trigger.RunAsync(timer: null!, cancellationToken: default);

        Assert.Equal(1, relay.CallCount);
    }
}
