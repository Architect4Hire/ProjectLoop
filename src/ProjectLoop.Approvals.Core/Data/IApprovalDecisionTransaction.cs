namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Owns the transaction boundary for applying a transition-valid
/// ApprovalRequest's terminal state together with the immutable
/// ApprovalDecision record that evidences it. Assumes the caller has already
/// confirmed the transition via <see cref="ApprovalStateTransition"/> — this
/// transaction only persists, it does not decide.
/// </summary>
public interface IApprovalDecisionTransaction
{
    Task<ApprovalDecision> ExecuteAsync(
        ApprovalRequest request,
        ApprovalRequestStatus outcome,
        string approverUserId,
        string? comments,
        DateTimeOffset decidedAtUtc,
        CancellationToken cancellationToken = default);
}
