namespace ProjectLoop.Audit.Core;

/// <summary>
/// Audit's own copy of the v1 payload for the ApprovalRequested integration
/// event, produced by the Approvals service. Each consuming service keeps an
/// independent copy of a contract it consumes rather than sharing the
/// producer's internal type.
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
