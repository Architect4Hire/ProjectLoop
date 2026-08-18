namespace ProjectLoop.Notifications.Core;

/// <summary>
/// Durable delivery/history record for one asynchronous notification raised
/// from a specific upstream integration event. SourceEventId is that
/// event's own stable EventId, not this row's key. RecipientUserId is a
/// stable identity reference rather than a persisted email address — per
/// the Identity rule, address resolution is a sender-time concern, not a
/// property of this domain record.
/// </summary>
public sealed class NotificationDelivery
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid SourceEventId { get; init; }

    public required string NotificationType { get; init; }

    public required string RecipientUserId { get; init; }

    public string? CorrelationId { get; init; }

    public required NotificationDeliveryStatus Status { get; set; }

    public int AttemptCount { get; set; }

    public required DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? SentAtUtc { get; set; }

    public DateTimeOffset? LastAttemptedAtUtc { get; set; }

    public string? LastError { get; set; }
}
