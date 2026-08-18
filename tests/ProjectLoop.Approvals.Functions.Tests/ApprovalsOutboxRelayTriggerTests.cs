using ProjectLoop.Approvals.Core;
using ProjectLoop.Approvals.Functions;
using Xunit;

namespace ProjectLoop.Approvals.Functions.Tests;

public class ApprovalsOutboxRelayTriggerTests
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
        var trigger = new ApprovalsOutboxRelayTrigger(relay);

        await trigger.RunAsync(timer: null!, cancellationToken: default);

        Assert.Equal(1, relay.CallCount);
    }
}
