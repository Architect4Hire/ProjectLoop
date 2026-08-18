namespace ProjectLoop.Identity.Core;

/// <summary>
/// Publishes one already-persisted, already-serialized outbox row to
/// Service Bus. Does not read the outbox or decide what to publish next —
/// the relay owns batching and retry.
/// </summary>
public interface IOutboxMessagePublisher
{
    Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}
