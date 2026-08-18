using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalApproveFacadeTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<ApprovalRequest> SeedRequestAsync(ApprovalsDbContext dbContext, Guid tenantId, ApprovalRequestStatus status)
    {
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
            Status = status,
        };
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();
        return request;
    }

    private static ApprovalApproveFacade CreateFacade(ApprovalsDbContext dbContext, ICurrentTenantContextAccessor tenantContextAccessor)
    {
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        return new ApprovalApproveFacade(tenantContextAccessor, requestRepository, transaction);
    }

    [Fact]
    public async Task ApproveAsync_Transitions_A_Pending_Request_To_Approved()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = await SeedRequestAsync(dbContext, tenantId, ApprovalRequestStatus.Pending);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id });

        Assert.True(result.IsSuccess);
        Assert.Equal(ApprovalRequestStatus.Approved, result.Result!.Status);
    }

    [Fact]
    public async Task ApproveAsync_Returns_Conflict_For_An_Already_Terminal_Request()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = await SeedRequestAsync(dbContext, tenantId, ApprovalRequestStatus.Approved);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.ApproveAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.Conflict, result.Error);
    }
}
