using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectLoop.Approvals.Core;
using ProjectLoop.Contracts;
using Xunit;

namespace ProjectLoop.Approvals.Core.Tests;

public class ApprovalDecisionTransactionOutboxTests
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
            CorrelationId = "correlation-1",
        };
        dbContext.ApprovalRequests.Add(request);
        await dbContext.SaveChangesAsync();
        return request;
    }

    [Fact]
    public async Task ExecuteAsync_Approved_Commits_Decision_State_And_ApprovalGranted_Outbox_Row_Together()
    {
        await using var dbContext = CreateDbContext();
        var request = await SeedPendingRequestAsync(dbContext);
        var transaction = new ApprovalDecisionTransaction(
            dbContext,
            new ApprovalRequestRepository(dbContext),
            new ApprovalDecisionRepository(dbContext));

        var decidedAtUtc = DateTimeOffset.UtcNow;
        var decision = await transaction.ExecuteAsync(request, ApprovalRequestStatus.Approved, "approver-1", "Looks good.", decidedAtUtc);

        var outboxRow = await dbContext.OutboxMessages.SingleAsync(m => m.EventType == "ApprovalGranted");
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalGrantedV1>>(outboxRow.Payload);

        Assert.NotNull(envelope);
        Assert.Equal(decision.Id, envelope!.Data.ApprovalDecisionId);
        Assert.Equal(request.Id, envelope.Data.ApprovalRequestId);
        Assert.Equal(request.TenantId, envelope.TenantId);
        Assert.Equal(request.CorrelationId, envelope.CorrelationId);
        Assert.Equal("approver-1", envelope.Data.ApproverUserId);
    }

    [Fact]
    public async Task ExecuteAsync_Rejected_Commits_Decision_State_And_ApprovalRejected_Outbox_Row_Together()
    {
        await using var dbContext = CreateDbContext();
        var request = await SeedPendingRequestAsync(dbContext);
        var transaction = new ApprovalDecisionTransaction(
            dbContext,
            new ApprovalRequestRepository(dbContext),
            new ApprovalDecisionRepository(dbContext));

        var decidedAtUtc = DateTimeOffset.UtcNow;
        var decision = await transaction.ExecuteAsync(request, ApprovalRequestStatus.Rejected, "approver-1", "Needs revisions.", decidedAtUtc);

        var outboxRow = await dbContext.OutboxMessages.SingleAsync(m => m.EventType == "ApprovalRejected");
        var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope<ApprovalRejectedV1>>(outboxRow.Payload);

        Assert.NotNull(envelope);
        Assert.Equal(decision.Id, envelope!.Data.ApprovalDecisionId);
        Assert.Equal(request.Id, envelope.Data.ApprovalRequestId);
        Assert.Equal(request.TenantId, envelope.TenantId);
        Assert.Equal("Needs revisions.", envelope.Data.Comments);
    }
}
