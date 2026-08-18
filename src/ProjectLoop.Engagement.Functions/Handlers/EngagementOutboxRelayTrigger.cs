using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Engagement.Core;

namespace ProjectLoop.Engagement.Functions;

/// <summary>
/// Thin timer entry point: no SQL or Service Bus logic lives here, only
/// delegation to the injected relay.
/// </summary>
public sealed class EngagementOutboxRelayTrigger
{
    private readonly IOutboxRelay _outboxRelay;

    public EngagementOutboxRelayTrigger(IOutboxRelay outboxRelay)
    {
        _outboxRelay = outboxRelay;
    }

    [Function(nameof(EngagementOutboxRelayTrigger))]
    public Task RunAsync(
        [TimerTrigger("%EngagementOutboxRelay:Schedule%")] TimerInfo timer,
        CancellationToken cancellationToken) =>
        _outboxRelay.RelayAsync(cancellationToken);
}
