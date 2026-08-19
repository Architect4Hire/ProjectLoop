namespace ProjectLoop.Audit.Core;

/// <summary>
/// Append-only durable record of one business/security-significant fact.
/// Audit is Project Loop's authoritative business audit ledger — separate
/// from application logs/traces — and a record is never updated or deleted
/// once written. BeforeMetadata/AfterMetadata are safe (non-sensitive)
/// projections of state — never raw document bodies, credentials, tokens,
/// or other sensitive payloads.
/// </summary>
public sealed class AuditRecord
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required string ActorUserId { get; init; }

    public required string Action { get; init; }

    public required string ResourceType { get; init; }

    public required Guid ResourceId { get; init; }

    public Guid? ResourceVersionId { get; init; }

    public required DateTimeOffset OccurredAtUtc { get; init; }

    public string? CorrelationId { get; init; }

    public string? CausationId { get; init; }

    public required string Source { get; init; }

    public string? BeforeMetadata { get; init; }

    public string? AfterMetadata { get; init; }
}
