using Microsoft.EntityFrameworkCore;

namespace ProjectLoop.Approvals.Core;

public sealed class ApprovalRequestRepository : IApprovalRequestRepository
{
    private readonly ApprovalsDbContext _dbContext;

    public ApprovalRequestRepository(ApprovalsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ApprovalRequest?> GetByIdAsync(Guid tenantId, Guid approvalRequestId, CancellationToken cancellationToken = default) =>
        _dbContext.ApprovalRequests
            .FirstOrDefaultAsync(r => r.Id == approvalRequestId && r.TenantId == tenantId, cancellationToken);

    public void ApplyDecision(ApprovalRequest request, ApprovalRequestStatus status)
    {
        if (_dbContext.Entry(request).State == EntityState.Detached)
        {
            _dbContext.ApprovalRequests.Attach(request);
        }

        request.Status = status;
    }
}
