namespace ProjectLoop.Approvals.Core;

public interface IApprovalApproveFacade
{
    Task<ApprovalDecisionFacadeResult> ApproveAsync(ApprovalDecisionRequest request, CancellationToken cancellationToken = default);
}
