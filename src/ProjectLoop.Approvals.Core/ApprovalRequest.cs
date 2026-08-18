namespace ProjectLoop.Approvals.Core;

/// <summary>
/// A request for an authorized approver to decide on an exact target
/// resource identity and, when the target is a document, an exact immutable
/// DocumentVersion. TargetVersionId is null for target types that are not
/// version-bound.
/// </summary>
public sealed class ApprovalRequest
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProjectId { get; init; }

    public required string TargetType { get; init; }

    public required Guid TargetId { get; init; }

    public Guid? TargetVersionId { get; init; }

    public required string RequestedByUserId { get; init; }

    public required DateTimeOffset RequestedAtUtc { get; init; }

    public required ApprovalRequestStatus Status { get; set; }

    public string? CorrelationId { get; init; }
}
