namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Durable idempotency record of one already-processed inbound integration
/// message. MessageId is the producer's stable EventId (from
/// IntegrationEventEnvelope.EventId) — its presence is what lets a consumer
/// recognize and safely ignore an at-least-once redelivered message rather
/// than re-applying its durable side effects.
/// </summary>
public sealed class InboxMessage
{
    public required Guid Id { get; init; }

    public required Guid MessageId { get; init; }

    public required string EventType { get; init; }

    public string? CorrelationId { get; init; }

    public required DateTimeOffset ProcessedAtUtc { get; init; }
}
