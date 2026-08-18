namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Pure domain rule deciding whether an ApprovalRequest may transition from
/// its current state to a proposed terminal state. Only Pending -> Approved
/// and Pending -> Rejected are valid; an already-terminal request can never
/// be decided again, so a repeated approve/reject against the same request
/// is always rejected by this rule rather than silently re-applied.
/// </summary>
public static class ApprovalStateTransition
{
    public static bool CanTransition(ApprovalRequestStatus current, ApprovalRequestStatus target) =>
        current == ApprovalRequestStatus.Pending &&
        target is ApprovalRequestStatus.Approved or ApprovalRequestStatus.Rejected;
}
