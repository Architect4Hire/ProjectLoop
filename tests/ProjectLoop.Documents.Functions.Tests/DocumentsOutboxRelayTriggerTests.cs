using ProjectLoop.Documents.Core;
using ProjectLoop.Documents.Functions;
using Xunit;

namespace ProjectLoop.Documents.Functions.Tests;

public class DocumentsOutboxRelayTriggerTests
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
        var trigger = new DocumentsOutboxRelayTrigger(relay);

        await trigger.RunAsync(timer: null!, cancellationToken: default);

        Assert.Equal(1, relay.CallCount);
    }
}
