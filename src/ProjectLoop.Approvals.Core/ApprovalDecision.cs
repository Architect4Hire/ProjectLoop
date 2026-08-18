namespace ProjectLoop.Approvals.Core;

/// <summary>
/// An immutable business record of a single approve/reject decision against
/// an exact ApprovalRequest. Once written, a decision row is never updated or
/// deleted — approval history is append-only evidence.
/// </summary>
public sealed class ApprovalDecision
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ApprovalRequestId { get; init; }

    public required string TargetType { get; init; }

    public required Guid TargetId { get; init; }

    public Guid? TargetVersionId { get; init; }

    public required string ApproverUserId { get; init; }

    public required ApprovalRequestStatus Decision { get; init; }

    public string? Comments { get; init; }

    public required DateTimeOffset DecidedAtUtc { get; init; }

    public string? CorrelationId { get; init; }
}
