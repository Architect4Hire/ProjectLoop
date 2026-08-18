using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRejectFacadeTests
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

    private static ApprovalRejectFacade CreateFacade(ApprovalsDbContext dbContext, ICurrentTenantContextAccessor tenantContextAccessor)
    {
        var requestRepository = new ApprovalRequestRepository(dbContext);
        var transaction = new ApprovalDecisionTransaction(dbContext, requestRepository, new ApprovalDecisionRepository(dbContext));
        return new ApprovalRejectFacade(tenantContextAccessor, requestRepository, transaction);
    }

    [Fact]
    public async Task RejectAsync_Transitions_A_Pending_Request_To_Rejected()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = await SeedRequestAsync(dbContext, tenantId, ApprovalRequestStatus.Pending);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.RejectAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id, Comments = "Needs revision." });

        Assert.True(result.IsSuccess);
        Assert.Equal(ApprovalRequestStatus.Rejected, result.Result!.Status);
    }

    [Fact]
    public async Task RejectAsync_Returns_Conflict_For_An_Already_Terminal_Request()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = await SeedRequestAsync(dbContext, tenantId, ApprovalRequestStatus.Rejected);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "approver-1" },
        };
        var facade = CreateFacade(dbContext, tenantContextAccessor);

        var result = await facade.RejectAsync(new ApprovalDecisionRequest { ApprovalRequestId = request.Id });

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalDecisionFacadeError.Conflict, result.Error);
    }
}
