namespace ProjectLoop.Engagement.Core;

/// <summary>
/// Engagement's own copy of the v1 payload for the ApprovalRejected
/// integration event, produced by the Approvals service. Each consuming
/// service keeps an independent copy of a contract it consumes rather than
/// sharing the producer's internal type.
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
