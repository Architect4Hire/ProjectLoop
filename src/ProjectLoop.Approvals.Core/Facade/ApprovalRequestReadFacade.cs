namespace ProjectLoop.Approvals.Core;

/// <summary>
/// Use-case orchestration for reading one approval request: resolves it
/// scoped to the caller's authenticated tenant. Tenant scoping in the
/// repository query is the authorization boundary — a request belonging to
/// another tenant is indistinguishable from a missing one.
/// </summary>
public sealed class ApprovalRequestReadFacade : IApprovalRequestReadFacade
{
    private readonly ICurrentTenantContextAccessor _tenantContextAccessor;
    private readonly IApprovalRequestRepository _approvalRequestRepository;

    public ApprovalRequestReadFacade(
        ICurrentTenantContextAccessor tenantContextAccessor,
        IApprovalRequestRepository approvalRequestRepository)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _approvalRequestRepository = approvalRequestRepository;
    }

    public async Task<ApprovalRequestReadFacadeResult> GetAsync(Guid approvalRequestId, CancellationToken cancellationToken = default)
    {
        var tenantContext = _tenantContextAccessor.Current;
        if (tenantContext is null)
        {
            return ApprovalRequestReadFacadeResult.Failure(ApprovalRequestReadFacadeError.NoTenantContext);
        }

        var request = await _approvalRequestRepository.GetByIdAsync(tenantContext.TenantId, approvalRequestId, cancellationToken);
        if (request is null)
        {
            return ApprovalRequestReadFacadeResult.Failure(ApprovalRequestReadFacadeError.NotFound);
        }

        var response = new ApprovalRequestResponse
        {
            Id = request.Id,
            ProjectId = request.ProjectId,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            TargetVersionId = request.TargetVersionId,
            Status = request.Status,
            RequestedByUserId = request.RequestedByUserId,
            RequestedAtUtc = request.RequestedAtUtc,
        };

        return ApprovalRequestReadFacadeResult.Success(response);
    }
}
