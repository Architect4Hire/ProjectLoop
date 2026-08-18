using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Identity.Core;

namespace ProjectLoop.Identity.Functions;

/// <summary>
/// Thin timer entry point: no SQL or Service Bus logic lives here, only
/// delegation to the injected relay.
/// </summary>
public sealed class IdentityOutboxRelayTrigger
{
    private readonly IOutboxRelay _outboxRelay;

    public IdentityOutboxRelayTrigger(IOutboxRelay outboxRelay)
    {
        _outboxRelay = outboxRelay;
    }

    [Function(nameof(IdentityOutboxRelayTrigger))]
    public Task RunAsync(
        [TimerTrigger("%IdentityOutboxRelay:Schedule%")] TimerInfo timer,
        CancellationToken cancellationToken) =>
        _outboxRelay.RelayAsync(cancellationToken);
}
