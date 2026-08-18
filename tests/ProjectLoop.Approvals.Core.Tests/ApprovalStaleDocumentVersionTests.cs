using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

/// <summary>
/// Proves an approval decision binds to the exact DocumentVersion its
/// ApprovalRequest was raised against. Approving the request for v3 must not
/// confer approval on a separately raised request for v4 of the same
/// document — each request/decision is scoped to its own TargetVersionId.
/// </summary>
public class ApprovalStaleDocumentVersionTests
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
    public async Task Approving_The_Request_For_V3_Does_Not_Approve_A_Later_Request_For_V4()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var documentId = Guid.NewGuid();
        var v3Id = Guid.NewGuid();
        var v4Id = Guid.NewGuid();

        var requestForV3 = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            TargetType = "DocumentVersion",
            TargetId = documentId,
            TargetVersionId = v3Id,
            RequestedByUserId = "requester-1",
            RequestedAtUtc = DateTimeOffset.UtcNow,
            Status = ApprovalRequestStatus.Pending,
        };
        var requestForV4 = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = requestForV3.ProjectId,
            TargetType = "DocumentVersion",
            TargetId = documentId,
            TargetVersionId = v4Id,
            RequestedByUserId = "requester-1",
            RequestedAtUtc = DateTimeOffset.UtcNow,
            Status = ApprovalRequestStatus.Pending,
        };
        dbContext.ApprovalRequests.AddRange(requestForV3, requestForV4);
        await dbContext.SaveChangesAsync();

        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        var approveFacade = new ApprovalApproveFacade(tenantContextAccessor, requestRepository, transaction);

        var result = await approveFacade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = requestForV3.Id });
        Assert.True(result.IsSuccess);

        var persistedV3Request = await dbContext.ApprovalRequests.SingleAsync(r => r.Id == requestForV3.Id);
        var persistedV4Request = await dbContext.ApprovalRequests.SingleAsync(r => r.Id == requestForV4.Id);

        Assert.Equal(ApprovalRequestStatus.Approved, persistedV3Request.Status);
        Assert.Equal(ApprovalRequestStatus.Pending, persistedV4Request.Status);

        var decision = await dbContext.ApprovalDecisions.SingleAsync(d => d.ApprovalRequestId == requestForV3.Id);
        Assert.Equal(v3Id, decision.TargetVersionId);
        Assert.NotEqual(v4Id, decision.TargetVersionId);
        Assert.False(await dbContext.ApprovalDecisions.AnyAsync(d => d.ApprovalRequestId == requestForV4.Id));
    }
}
