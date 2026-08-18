namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Identifies the exact ApprovalRequest to decide, plus the approver's
/// optional comments. The decision outcome (approve/reject) is carried by
/// which operation is invoked, not by a field on this contract. Tenant and
/// approver identity are never taken from this contract — they are always
/// derived server-side from the authenticated
/// <see cref="ICurrentTenantContextAccessor"/>.
/// </summary>
public sealed class ApprovalDecisionRequest
{
    public required Guid ApprovalRequestId { get; init; }

    public string? Comments { get; init; }
}
