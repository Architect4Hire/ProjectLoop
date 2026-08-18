namespace ProjectLoop.Approvals.Core;

public interface IApprovalRequestRepository
{
    /// <summary>
    /// Returns the ApprovalRequest with the given Id scoped to the given
    /// tenant, or null when it does not exist or belongs to a different
    /// tenant. Tenant scoping is applied in the query itself so an
    /// unauthorized cross-tenant Id can never be distinguished from a
    /// missing one.
    /// </summary>
    Task<ApprovalRequest?> GetByIdAsync(Guid tenantId, Guid approvalRequestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the ApprovalRequest's terminal Status transition on an
    /// already-tracked request. Assumes the caller has already confirmed the
    /// transition is valid via <see cref="ApprovalStateTransition"/> — this
    /// method only persists, it does not decide. Does not commit.
    /// </summary>
    void ApplyDecision(ApprovalRequest request, ApprovalRequestStatus status);

    /// <summary>
    /// Stages a new ApprovalRequest for insertion. Does not commit.
    /// </summary>
    void Add(ApprovalRequest request);
}
