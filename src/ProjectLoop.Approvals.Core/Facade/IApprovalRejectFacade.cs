namespace ProjectLoop.Approvals.Core;

public interface IApprovalRejectFacade
{
    Task<ApprovalDecisionFacadeResult> RejectAsync(ApprovalDecisionRequest request, CancellationToken cancellationToken = default);
}
