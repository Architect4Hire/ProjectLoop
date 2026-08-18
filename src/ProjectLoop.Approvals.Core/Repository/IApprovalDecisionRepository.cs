namespace ProjectLoop.Approvals.Core;

public interface IApprovalDecisionRepository
{
    /// <summary>
    /// Stages a new immutable ApprovalDecision for persistence. Does not
    /// commit — the caller's transaction boundary decides when changes are
    /// saved.
    /// </summary>
    Task AddAsync(ApprovalDecision decision, CancellationToken cancellationToken = default);
}
