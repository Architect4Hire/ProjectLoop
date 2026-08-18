using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalRequestRepositoryTests
{
    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static ApprovalRequest CreateRequest(Guid tenantId, Guid id)
    {
        var now = DateTimeOffset.UtcNow;

        return new ApprovalRequest
        {
            Id = id,
            TenantId = tenantId,
            ProjectId = Guid.NewGuid(),
            TargetType = "DocumentVersion",
            TargetId = Guid.NewGuid(),
            TargetVersionId = Guid.NewGuid(),
            RequestedByUserId = "user-1",
            RequestedAtUtc = now,
            Status = ApprovalRequestStatus.Pending,
        };
    }

    [Fact]
    public async Task GetByIdAsync_Returns_The_Request_For_The_Owning_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var request = CreateRequest(tenantId, Guid.NewGuid());
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var repository = new ApprovalRequestRepository(dbContext);
        var found = await repository.GetByIdAsync(tenantId, request.Id);

        Assert.NotNull(found);
        Assert.Equal(request.Id, found!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_For_A_Different_Tenant()
    {
        await using var dbContext = CreateDbContext();
        var ownerTenantId = Guid.NewGuid();
        var callerTenantId = Guid.NewGuid();
        var request = CreateRequest(ownerTenantId, Guid.NewGuid());
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();

        var repository = new ApprovalRequestRepository(dbContext);
        var found = await repository.GetByIdAsync(callerTenantId, request.Id);

        Assert.Null(found);
    }
}
