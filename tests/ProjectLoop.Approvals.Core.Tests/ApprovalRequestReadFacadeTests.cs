using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRequestReadFacadeTests
{
    private sealed class FakeTenantContextAccessor : ICurrentTenantContextAccessor
    {
        public ITenantContext? Current { get; set; }
    }

    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<ApprovalRequest> SeedAsync(ApprovalsDbContext dbContext, Guid tenantId)
    {
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            TargetType = "DocumentVersion",
            TargetId = Guid.NewGuid(),
            TargetVersionId = Guid.NewGuid(),
            RequestedByUserId = "user-1",
            RequestedAtUtc = DateTimeOffset.UtcNow,
            Status = ApprovalRequestStatus.Pending,
        };
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();
        return request;
    }

    [Fact]
    public async Task GetAsync_Returns_Failure_When_No_Tenant_Context()
    {
        await using var dbContext = CreateDbContext();
        var facade = new ApprovalRequestReadFacade(new FakeTenantContextAccessor(), new ApprovalRequestRepository(dbContext));

        var result = await facade.GetAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalRequestReadFacadeError.NoTenantContext, result.Error);
    }

    [Fact]
    public async Task GetAsync_Returns_The_Request_For_The_Caller_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = await SeedAsync(dbContext, tenantId);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = tenantId, UserId = "user-1" },
        };
        var facade = new ApprovalRequestReadFacade(tenantContextAccessor, new ApprovalRequestRepository(dbContext));

        var result = await facade.GetAsync(request.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(request.Id, result.Response!.Id);
    }

    [Fact]
    public async Task GetAsync_Returns_NotFound_For_A_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var ownerTenantId = Guid.NewGuid();
        var callerTenantId = Guid.NewGuid();
        var request = await SeedAsync(dbContext, ownerTenantId);
        var tenantContextAccessor = new FakeTenantContextAccessor
        {
            Current = new TenantContext { TenantId = callerTenantId, UserId = "caller-user" },
        };
        var facade = new ApprovalRequestReadFacade(tenantContextAccessor, new ApprovalRequestRepository(dbContext));

        var result = await facade.GetAsync(request.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ApprovalRequestReadFacadeError.NotFound, result.Error);
    }
}
