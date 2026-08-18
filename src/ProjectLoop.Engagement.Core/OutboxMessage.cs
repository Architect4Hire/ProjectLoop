namespace ProjectLoop.Engagement.Core;

/// <summary>
/// A service-local outbox row committed in the same transaction as the
/// business state change it describes. EventId is the stable identity
/// propagated to consumers and is distinct from Id, the row's own key.
/// </summary>
public sealed class OutboxMessage
{
    public required Guid Id { get; init; }

    public required Guid EventId { get; init; }

    public required string EventType { get; init; }

    public required int EventVersion { get; init; }

    public required string Payload { get; init; }

    public string? CorrelationId { get; init; }

    public required OutboxMessageStatus Status { get; set; }

    public int AttemptCount { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? ProcessedAtUtc { get; set; }

    public DateTimeOffset? LastAttemptedAtUtc { get; set; }

    public string? LastError { get; set; }
}
