using Microsoft.Azure.Functions.Worker;
using ProjectLoop.Documents.Core;

namespace ProjectLoop.Documents.Functions;

/// <summary>
/// Thin timer entry point: no SQL or Service Bus logic lives here, only
/// delegation to the injected relay.
/// </summary>
public sealed class DocumentsOutboxRelayTrigger
{
    private readonly IOutboxRelay _outboxRelay;

    public DocumentsOutboxRelayTrigger(IOutboxRelay outboxRelay)
    {
        _outboxRelay = outboxRelay;
    }

    [Function(nameof(DocumentsOutboxRelayTrigger))]
    public Task RunAsync(
        [TimerTrigger("%DocumentsOutboxRelay:Schedule%")] TimerInfo timer,
        CancellationToken cancellationToken) =>
        _outboxRelay.RelayAsync(cancellationToken);
}
