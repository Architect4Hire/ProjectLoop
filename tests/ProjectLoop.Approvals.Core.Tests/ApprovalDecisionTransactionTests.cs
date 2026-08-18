using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalDecisionTransactionTests
{
    private static ApprovalsDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApprovalsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<ApprovalRequest> SeedPendingRequestAsync(ApprovalsDbContext dbContext)
    {
        var request = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
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
        return request;
    }

    [Fact]
    public async Task ExecuteAsync_Applies_Terminal_State_And_Appends_One_Decision()
    {
        await using var dbContext = CreateDbContext();
        var request = await SeedPendingRequestAsync(dbContext);
        var transaction = new ApprovalDecisionTransaction(
            dbContext,
            new ApprovalRequestRepository(dbContext),
            new ApprovalDecisionRepository(dbContext));
        var now = DateTimeOffset.UtcNow;

        var decision = await transaction.ExecuteAsync(request, ApprovalRequestStatus.Approved, "approver-1", "Looks good.", now);

        var persistedRequest = await dbContext.ApprovalRequests.SingleAsync(r => r.Id == request.Id);
        Assert.Equal(ApprovalRequestStatus.Approved, persistedRequest.Status);

        var persistedDecision = await dbContext.ApprovalDecisions.SingleAsync(d => d.Id == decision.Id);
        Assert.Equal(ApprovalRequestStatus.Approved, persistedDecision.Decision);
        Assert.Equal(request.Id, persistedDecision.ApprovalRequestId);
        Assert.Equal("approver-1", persistedDecision.ApproverUserId);
    }

    [Fact]
    public async Task ExecuteAsync_Does_Not_Enqueue_An_Outbox_Message()
    {
        await using var dbContext = CreateDbContext();
        var request = await SeedPendingRequestAsync(dbContext);
        var transaction = new ApprovalDecisionTransaction(
            dbContext,
            new ApprovalRequestRepository(dbContext),
            new ApprovalDecisionRepository(dbContext));

        await transaction.ExecuteAsync(request, ApprovalRequestStatus.Rejected, "approver-1", null, DateTimeOffset.UtcNow);

        Assert.False(await dbContext.OutboxMessages.AnyAsync());
    }
}
