using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

/// <summary>
/// Proves Tenant A cannot decide an ApprovalRequest owned by Tenant B, even
/// when Tenant A supplies the exact, valid ApprovalRequest Id. Tenant
/// scoping in the repository lookup is the authorization boundary — a
/// cross-tenant request is treated identically to a missing one.
/// </summary>
public class ApprovalCrossTenantDenialTests
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
    public async Task ApproveAsync_Denies_A_Caller_From_A_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var ownerTenantId = Guid.NewGuid();
        var callerTenantId = Guid.NewGuid();
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = ownerTenantId,
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
            Current = new TenantContext { TenantId = callerTenantId, UserId = "attacker-user" },
        };
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        var approveFacade = new ApprovalApproveFacade(tenantContextAccessor, requestRepository, transaction);

        var result = await approveFacade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.RequestNotFound, result.Error);

        var persistedRequest = await dbContext.ApprovalRequests.SingleAsync(r => r.Id == request.Id);
        Assert.Equal(ApprovalRequestStatus.Pending, persistedRequest.Status);
        Assert.False(await dbContext.ApprovalDecisions.AnyAsync(d => d.ApprovalRequestId == request.Id));
    }

    [Fact]
    public async Task RejectAsync_Denies_A_Caller_From_A_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var ownerTenantId = Guid.NewGuid();
        var callerTenantId = Guid.NewGuid();
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = ownerTenantId,
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
            Current = new TenantContext { TenantId = callerTenantId, UserId = "attacker-user" },
        };
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        var rejectFacade = new ApprovalRejectFacade(tenantContextAccessor, requestRepository, transaction);

        var result = await rejectFacade.RejectAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.RequestNotFound, result.Error);
    }
}
