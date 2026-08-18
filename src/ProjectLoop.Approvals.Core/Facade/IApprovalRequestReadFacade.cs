namespace ProjectLoop.Approvals.Core;

public interface IApprovalRequestReadFacade
{
    Task<ApprovalRequestReadFacadeResult> GetAsync(Guid approvalRequestId, CancellationToken cancellationToken = default);
}
