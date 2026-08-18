namespace ProjectLoop.Approvals.Core;

/// <summary>
/// v1 payload for the ApprovalRejected integration event: the completed fact
/// that an ApprovalRequest was decided Rejected. Carries the exact target
/// resource and, when version-bound, the exact immutable version the
/// decision applies to — never the target's own content or Blob references.
/// </summary>
public sealed record ApprovalRejectedV1(
    Guid ApprovalDecisionId,
    Guid ApprovalRequestId,
    Guid TenantId,
    Guid ProjectId,
    string TargetType,
    Guid TargetId,
    Guid? TargetVersionId,
    string ApproverUserId,
    string? Comments,
    DateTimeOffset DecidedAtUtc);
