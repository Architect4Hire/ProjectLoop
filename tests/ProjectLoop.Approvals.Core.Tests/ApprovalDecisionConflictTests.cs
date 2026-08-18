using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

/// <summary>
/// Proves a terminal ApprovalRequest cannot be decided again — through
/// either operation — and that the original decision row is left untouched
/// by the rejected repeat attempt.
/// </summary>
public class ApprovalDecisionConflictTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Approving_An_Already_Approved_Request_Is_Rejected_And_Leaves_The_Prior_Decision_Unchanged()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            TargetType = "DocumentVersion",
            TargetId = Guid.NewGuid(),
            TargetVersionId = Guid.NewGuid(),
            RequestedByUserId = "requester-1",
            RequestedAtUtc = DateTimeOffset.UtcNow,
            Status = ApprovalRequestStatus.Pending,
        };
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        var approveFacade = new ApprovalApproveFacade(tenantContextAccessor, requestRepository, transaction);
        var rejectFacade = new ApprovalRejectFacade(tenantContextAccessor, requestRepository, transaction);

        var firstDecision = await approveFacade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id, Comments = "First pass." });
        Assert.True(firstDecision.IsSuccess);

        var repeatApprove = await approveFacade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id, Comments = "Second pass." });
        var repeatReject = await rejectFacade.RejectAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id, Comments = "Trying to flip it." });

        Assert.False(repeatApprove.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.Conflict, repeatApprove.Error);
        Assert.False(repeatReject.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.Conflict, repeatReject.Error);

        var persistedRequest = await dbContext.ApprovalRequests.SingleAsync(r => r.Id == request.Id);
        Assert.Equal(ApprovalRequestStatus.Approved, persistedRequest.Status);

        var decisions = await dbContext.ApprovalDecisions.Where(d => d.ApprovalRequestId == request.Id).ToListAsync();
        var decision = Assert.Single(decisions);
        Assert.Equal(firstDecision.Result!.DecisionId, decision.Id);
        Assert.Equal("First pass.", decision.Comments);
        Assert.Equal(ApprovalRequestStatus.Approved, decision.Decision);
    }
}
