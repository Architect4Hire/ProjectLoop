namespace ProjectLoop.Approvals.Core;

/// <summary>
/// v1 payload for the ApprovalRequested integration event: the completed
/// fact that a new ApprovalRequest was raised against an exact target
/// resource and, when version-bound, an exact immutable version. Carries
/// only notification-safe identifiers and metadata — no document content or
/// Blob references.
/// </summary>
public sealed record ApprovalRequestedV1(
    Guid ApprovalRequestId,
    Guid TenantId,
    Guid ProjectId,
    string TargetType,
    Guid TargetId,
    Guid? TargetVersionId,
    string RequestedByUserId,
    DateTimeOffset RequestedAtUtc);
