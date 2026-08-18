namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Use-case validation and orchestration for rejecting an ApprovalRequest:
/// the target request must exist within the caller's authenticated tenant
/// and must be eligible for a Pending -> Rejected transition per
/// <see cref="ApprovalStateTransition"/>.
/// </summary>
public sealed class ApprovalRejectFacade : IApprovalRejectFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IApprovalRequestRepository _approvalRequestRepository;
    private readonly IApprovalDecisionTransaction _decisionTransaction;
    private readonly TimeProvider _timeProvider;

    public ApprovalRejectFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IApprovalRequestRepository approvalRequestRepository,
        IApprovalDecisionTransaction decisionTransaction,
        TimeProvider? timeProvider = null)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _approvalRequestRepository = approvalRequestRepository;
        _decisionTransaction = decisionTransaction;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<ApprovalDecisionFacadeResult> RejectAsync(ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return ApprovalDecisionFacadeResult.Failure(ApprovalDecisionFacadeError.NoTenantContext);
        }

        var approvalRequest = await _approvalRequestRepository.GetByIdAsync(tenantContext.TenantId, request.ApprovalRequestId, cancellationToken);
        if (approvalRequest is null)
        {
            return ApprovalDecisionFacadeResult.Failure(ApprovalDecisionFacadeError.RequestNotFound);
        }

        if (!ApprovalStateTransition.CanTransition(approvalRequest.Status, ApprovalRequestStatus.Rejected))
        {
            return ApprovalDecisionFacadeResult.Failure(ApprovalDecisionFacadeError.Conflict);
        }

        var now = _timeProvider.GetUtcNow();
        var decision = await _decisionTransaction.ExecuteAsync(
            approvalRequest,
            ApprovalRequestStatus.Rejected,
            tenantContext.UserId,
            request.Comments,
            now,
            cancellationToken);

        return ApprovalDecisionFacadeResult.Success(new ApprovalDecisionResult
        {
            ApprovalRequestId = approvalRequest.Id,
            DecisionId = decision.Id,
            Status = approvalRequest.Status,
            ApproverUserId = decision.ApproverUserId,
            DecidedAtUtc = decision.DecidedAtUtc,
        });
    }
}
