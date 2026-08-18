using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Approvals.Core;

namespace ProjectLoop.Approvals.Functions;

/// <summary>
/// Thin timer entry point: no SQL or Service Bus logic lives here, only
/// delegation to the injected relay.
/// </summary>
public sealed class ApprovalsOutboxRelayTrigger
{
    private readonly IOutboxRelay _outboxRelay;

    public ApprovalsOutboxRelayTrigger(IOutboxRelay outboxRelay)
    {
        _outboxRelay = outboxRelay;
    }

    [Function(nameof(ApprovalsOutboxRelayTrigger))]
    public Task RunAsync(
        [TimerTrigger("%ApprovalsOutboxRelay:Schedule%")] TimerInfo timer,
        CancellationToken cancellationToken) =>
        _outboxRelay.RelayAsync(cancellationToken);
}
